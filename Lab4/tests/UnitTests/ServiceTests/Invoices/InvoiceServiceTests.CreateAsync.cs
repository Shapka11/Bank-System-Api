using AutoBogus;
using BankSystemApi.Application.Contracts.Invoices.Models;
using BankSystemApi.Application.Contracts.Invoices.Operations;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Invoices;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Domain.Invoices.States;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using FluentAssertions;
using UnitTests.MockExtensions;
using UnitTests.Models;

namespace UnitTests.ServiceTests.Invoices;

public sealed partial class InvoiceServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldReturnDto()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();

        Account receiverAccount = new AutoFaker<Account>().Generate();
        var amount = new Money(_faker.Finance.Amount());

        CreateInvoice.Request request = new(
            user.AuthorizationId,
            senderAccount.Id.Value,
            receiverAccount.Id.Value,
            amount.Value);

        DateTimeOffset currentTime = _faker.Date.RecentOffset();
        _dateTimeProvider.Setup(time => time.Current).Returns(currentTime);

        var invoice = new Invoice(
            InvoiceId.Default,
            senderAccount.Id,
            receiverAccount.Id,
            amount,
            new CreatedInvoiceState(),
            currentTime,
            currentTime);
        var expectedInvoiceId = new InvoiceId(_faker.Random.Long(0));

        var operationSenderAccount = new InvoiceIssuedHistoryOperation(
            HistoryOperationId.Default,
            senderAccount.Id,
            invoice.Id,
            currentTime);
        var expectedIssuedOperationId = new HistoryOperationId(_faker.Random.Long(0));

        var operationReceiverAccount = new InvoiceReceivedHistoryOperation(
            HistoryOperationId.Default,
            receiverAccount.Id,
            invoice.Id,
            currentTime);
        var expectedReceivedOperationId = new HistoryOperationId(_faker.Random.Long(0));

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id, senderAccount)
            .SetupQueryAccountById(receiverAccount.Id, receiverAccount);

        _persistenceContext.InvoicesRepository
            .SetupAddInvoice(invoice, expectedInvoiceId);

        _persistenceContext.HistoryOperationsRepository
            .SetupAddHistoryOperation(
                [operationSenderAccount, operationReceiverAccount],
                [expectedIssuedOperationId, expectedReceivedOperationId]);

        var invoiceDto = new InvoiceDto(
            expectedInvoiceId.Value,
            invoice.SenderAccountId.Value,
            invoice.ReceiverAccountId.Value,
            invoice.Amount.Value,
            InvoiceStatusDto.Created,
            invoice.CreatedAt,
            invoice.UpdatedAt);

        _serviceMetrics.Setup(m => m.IncInvoiceCreated());

        // Act
        CreateInvoice.Response response = await _invoiceService.CreateAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<CreateInvoice.Response.Success>()
            .Which.Invoice.Should()
            .BeEquivalentTo(invoiceDto);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();

        Account receiverAccount = new AutoFaker<Account>().Generate();
        var amount = new Money(_faker.Finance.Amount());

        CreateInvoice.Request request = new(
            user.AuthorizationId,
            senderAccount.Id.Value,
            receiverAccount.Id.Value,
            amount.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId);

        // Act
        CreateInvoice.Response response = await _invoiceService.CreateAsync(request, default);

        // Assert
        response.Should().BeOfType<CreateInvoice.Response.Unauthorized>();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSenderAccountNotFound_WhenSenderAccountNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();

        Account receiverAccount = new AutoFaker<Account>().Generate();
        var amount = new Money(_faker.Finance.Amount());

        CreateInvoice.Request request = new(
            user.AuthorizationId,
            senderAccount.Id.Value,
            receiverAccount.Id.Value,
            amount.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id);

        // Act
        CreateInvoice.Response response = await _invoiceService.CreateAsync(request, default);

        // Assert
        response.Should().BeOfType<CreateInvoice.Response.SenderAccountNotFound>();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnReceiverAccountNotFound_WhenReceiverAccountNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();

        Account receiverAccount = new AutoFaker<Account>().Generate();
        var amount = new Money(_faker.Finance.Amount());

        CreateInvoice.Request request = new(
            user.AuthorizationId,
            senderAccount.Id.Value,
            receiverAccount.Id.Value,
            amount.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id, senderAccount)
            .SetupQueryAccountById(receiverAccount.Id);

        // Act
        CreateInvoice.Response response = await _invoiceService.CreateAsync(request, default);

        // Assert
        response.Should().BeOfType<CreateInvoice.Response.ReceiverAccountNotFound>();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnForbidden_WhenSenderAccountNotUsers()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<Account>().Generate();

        Account receiverAccount = new AutoFaker<Account>().Generate();
        var amount = new Money(_faker.Finance.Amount());

        CreateInvoice.Request request = new(
            user.AuthorizationId,
            senderAccount.Id.Value,
            receiverAccount.Id.Value,
            amount.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id, senderAccount);

        // Act
        CreateInvoice.Response response = await _invoiceService.CreateAsync(request, default);

        // Assert
        response.Should().BeOfType<CreateInvoice.Response.Forbidden>();
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenRequestIncorrect()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();

        Account receiverAccount = new AutoFaker<Account>().Generate();

        CreateInvoice.Request request = new(
            user.AuthorizationId,
            senderAccount.Id.Value,
            receiverAccount.Id.Value,
            _faker.Finance.Amount(max: -1));

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id, senderAccount)
            .SetupQueryAccountById(receiverAccount.Id, receiverAccount);

        // Act
        Func<Task> result = async () => await _invoiceService.CreateAsync(request, default);

        // Assert
        await result.Should().ThrowAsync<ArgumentException>();
    }
}