using AutoBogus;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Application.Abstractions.Persistence.Results;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Invoices.States;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using BankSystemApi.Invoices.Grpc;
using Bogus;
using FluentAssertions;
using FluentAssertions.Specialized;
using Grpc.Core;
using IntegrationalTests.Fixtures;
using IntegrationalTests.Models;
using Microsoft.Extensions.DependencyInjection;
using Account = BankSystemApi.Domain.Accounts.Account;
using Invoice = BankSystemApi.Domain.Invoices.Invoice;
using User = BankSystemApi.Domain.Users.User;

namespace IntegrationalTests.ControllerTests;

[Collection(nameof(WebApplicationCollectionFixture))]
public sealed class InvoiceControllerTests : IAsyncDisposable
{
    private readonly AsyncServiceScope _scope;
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly InvoiceService.InvoiceServiceClient _invoiceServiceClient;

    private readonly Faker _faker = new()
    {
        Random = new Randomizer(42),
    };

    public InvoiceControllerTests(WebApplicationFixture fixture)
    {
        _scope = fixture.Services.CreateAsyncScope();
        _userRepository = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _accountRepository = _scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        _invoiceRepository = _scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
        _invoiceServiceClient = new InvoiceService.InvoiceServiceClient(fixture.CreateChannel());
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task CreateInvoiceAsync_ShouldCreateInvoice()
    {
        // Arrange
        User senderUser = await SeedUser();
        Account senderAccount = await SeedAccount(senderUser.Id);
        Account receiverAccount = await SeedAccount(new UserId(_faker.Random.Long()));
        var amount = new Money(_faker.Finance.Amount());

        var request = new ProtoCreateInvoiceRequest(
            senderUser.AuthorizationId.ToString(),
            senderAccount.Id.Value,
            receiverAccount.Id.Value,
            new Google.Type.Money { DecimalValue = amount.Value });

        // Act
        ProtoCreateInvoiceResponse response = await _invoiceServiceClient.CreateAsync(request, default);

        // Assert
        response.Invoice.Amount.DecimalValue.Should().Be(amount.Value);
        response.Invoice.SenderAccountId.Should().Be(senderAccount.Id.Value);
        response.Invoice.ReceiverAccountId.Should().Be(receiverAccount.Id.Value);
        response.Invoice.Status.Should().Be(ProtoInvoiceStatus.Created);
    }

    [Fact]
    public async Task CreateInvoiceAsync_ShouldThrowException_WhenSenderAccountNotUsers()
    {
        // Arrange
        User senderUser = await SeedUser();
        Account senderAccount = await SeedAccount(new UserId(_faker.Random.Long()));
        Account receiverAccount = await SeedAccount(new UserId(_faker.Random.Long()));
        var amount = new Money(_faker.Finance.Amount());

        var request = new ProtoCreateInvoiceRequest(
            senderUser.AuthorizationId.ToString(),
            senderAccount.Id.Value,
            receiverAccount.Id.Value,
            new Google.Type.Money { DecimalValue = amount.Value });

        // Act
        Func<Task<ProtoCreateInvoiceResponse>> responseFunc = async () =>
            await _invoiceServiceClient.CreateAsync(request, default);

        // Assert
        ExceptionAssertions<RpcException> response = await responseFunc.Should().ThrowAsync<RpcException>();
        response.Which.StatusCode.Should().Be(StatusCode.PermissionDenied);
    }

    [Fact]
    public async Task PayInvoiceAsync_ShouldPayInvoice()
    {
        // Arrange
        User receiverUser = await SeedUser();
        Account senderAccount = await SeedAccount(new UserId(_faker.Random.Long()));
        Account receiverAccount = await SeedAccount(receiverUser.Id);
        var invoiceAmount = new Money(_faker.Finance.Amount(max: receiverAccount.Balance.Value));
        Invoice invoice = await SeedInvoice(
            senderAccount.Id,
            receiverAccount.Id,
            new ApprovedInvoiceState(),
            invoiceAmount);

        var request = new ProtoPayInvoiceRequest(
            receiverUser.AuthorizationId.ToString(),
            invoice.Id.Value);

        // Act
        ProtoPayInvoiceResponse response = await _invoiceServiceClient.PayAsync(request, default);

        // Assert
        response.Invoice.Status.Should().Be(ProtoInvoiceStatus.Paid);
    }

    [Fact]
    public async Task PayInvoiceAsync_ShouldThrowException_WhenInvalidInvoiceState()
    {
        // Arrange
        User receiverUser = await SeedUser();
        Account senderAccount = await SeedAccount(new UserId(_faker.Random.Long()));
        Account receiverAccount = await SeedAccount(receiverUser.Id);
        var invoiceAmount = new Money(_faker.Finance.Amount(max: receiverAccount.Balance.Value));
        Invoice invoice = await SeedInvoice(
            senderAccount.Id,
            receiverAccount.Id,
            new PaidInvoiceState(),
            invoiceAmount);

        var request = new ProtoPayInvoiceRequest(
            receiverUser.AuthorizationId.ToString(),
            invoice.Id.Value);

        // Act
        Func<Task<ProtoPayInvoiceResponse>> responseFunc = async () =>
            await _invoiceServiceClient.PayAsync(request, default);

        // Assert
        ExceptionAssertions<RpcException> response = await responseFunc.Should().ThrowAsync<RpcException>();
        response.Which.StatusCode.Should().Be(StatusCode.FailedPrecondition);
    }

    [Fact]
    public async Task PayInvoiceAsync_ShouldThrowException_WhenReceiverAccountNotUsers()
    {
        // Arrange
        User receiverUser = await SeedUser();
        Account senderAccount = await SeedAccount(new UserId(_faker.Random.Long()));
        Account receiverAccount = await SeedAccount(new UserId(_faker.Random.Long()));
        var invoiceAmount = new Money(_faker.Finance.Amount(max: receiverAccount.Balance.Value));
        Invoice invoice = await SeedInvoice(
            senderAccount.Id,
            receiverAccount.Id,
            new ApprovedInvoiceState(),
            invoiceAmount);

        var request = new ProtoPayInvoiceRequest(
            receiverUser.AuthorizationId.ToString(),
            invoice.Id.Value);

        // Act
        Func<Task<ProtoPayInvoiceResponse>> responseFunc = async () =>
            await _invoiceServiceClient.PayAsync(request, default);

        // Assert
        ExceptionAssertions<RpcException> response = await responseFunc.Should().ThrowAsync<RpcException>();
        response.Which.StatusCode.Should().Be(StatusCode.PermissionDenied);
    }

    [Fact]
    public async Task PayInvoiceAsync_ShouldThrowException_WhenWithdrawalError()
    {
        // Arrange
        User receiverUser = await SeedUser();
        Account senderAccount = await SeedAccount(new UserId(_faker.Random.Long()));
        Account receiverAccount = await SeedAccount(receiverUser.Id);
        var invoiceAmount = new Money(_faker.Finance.Amount(min: receiverAccount.Balance.Value));
        Invoice invoice = await SeedInvoice(
            senderAccount.Id,
            receiverAccount.Id,
            new CreatedInvoiceState(),
            invoiceAmount);

        var request = new ProtoPayInvoiceRequest(
            receiverUser.AuthorizationId.ToString(),
            invoice.Id.Value);

        // Act
        Func<Task<ProtoPayInvoiceResponse>> responseFunc = async () =>
            await _invoiceServiceClient.PayAsync(request, default);

        // Assert
        ExceptionAssertions<RpcException> response = await responseFunc.Should().ThrowAsync<RpcException>();
        response.Which.StatusCode.Should().Be(StatusCode.FailedPrecondition);
    }

    private async Task<User> SeedUser()
    {
        User user = new AutoFaker<User>().Generate();
        var result = (AddUserResult.Success)await _userRepository.TryAddAsync([user], default);

        return result.User;
    }

    private async Task<Account> SeedAccount(UserId userId)
    {
        Account account = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, userId)
            .RuleFor(a => a.Number, new AccountNumber(Guid.NewGuid().ToString()))
            .Generate()
            .MapToDomain();
        return await _accountRepository.AddAsync([account], default).FirstAsync();
    }

    private async Task<Invoice> SeedInvoice(
        AccountId senderAccountId,
        AccountId receiverAccountId,
        IInvoiceState invoiceState,
        Money amount)
    {
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccountId)
            .RuleFor(i => i.ReceiverAccountId, receiverAccountId)
            .RuleFor(i => i.State, invoiceState)
            .RuleFor(i => i.Amount, amount)
            .Generate()
            .MapToDomain();
        return await _invoiceRepository.AddAsync([invoice], default).FirstAsync();
    }
}