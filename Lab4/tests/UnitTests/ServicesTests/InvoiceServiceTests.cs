using AutoBogus;
using BankSystemApi.Application.Abstractions.Metrics;
using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Contracts.Invoices.Models;
using BankSystemApi.Application.Contracts.Invoices.Operations;
using BankSystemApi.Application.Services;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Invoices;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Domain.Invoices.States;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using Itmo.Dev.Platform.Common.DateTime;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UnitTests.MockExtensions;
using UnitTests.Mocks;
using UnitTests.Models;

namespace UnitTests.ServicesTests;

public sealed class InvoiceServiceTests : IAsyncLifetime
{
    private readonly MockPersistenceContext _persistenceContext = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProvider = new(MockBehavior.Strict);
    private readonly Mock<IPersistenceTransactionProvider> _transactionProvider = new(MockBehavior.Strict);
    private readonly Mock<IServiceMetrics> _serviceMetrics = new(MockBehavior.Strict);

    private readonly InvoiceService _invoiceService;

    private readonly Faker _faker = new();

    public InvoiceServiceTests()
    {
        _transactionProvider.SetupDefaultTransaction();

        _invoiceService = new InvoiceService(
            _persistenceContext,
            _dateTimeProvider.Object,
            _transactionProvider.Object,
            NullLogger<InvoiceService>.Instance,
            _serviceMetrics.Object);
    }

    public static IEnumerable<object[]> GetTerminalStates()
    {
        yield return [new PaidInvoiceState()];
        yield return [new RevokedInvoiceState()];
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        _persistenceContext.VerifyAll();
        _dateTimeProvider.VerifyAll();
        _serviceMetrics.VerifyAll();

        return Task.CompletedTask;
    }

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

    [Fact]
    public async Task PayAsync_ShouldReturnDto()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<Account>().Generate();
        var invoiceAmount = new Money(_faker.Finance.Amount());
        Account receiverAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .RuleFor(a => a.Balance, invoiceAmount)
            .Generate()
            .MapToDomain();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .RuleFor(i => i.Amount, invoiceAmount)
            .Generate()
            .MapToDomain();

        PayInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        DateTimeOffset currentTime = _faker.Date.RecentOffset();
        _dateTimeProvider.Setup(time => time.Current).Returns(currentTime);

        var operationSenderAccount = new InvoicePaymentSentHistoryOperation(
            HistoryOperationId.Default,
            senderAccount.Id,
            invoice.Amount,
            invoice.Id,
            currentTime);
        var expectedIssuedOperationId = new HistoryOperationId(_faker.Random.Long(0));

        var operationReceiverAccount = new InvoicePaymentReceivedHistoryOperation(
            HistoryOperationId.Default,
            receiverAccount.Id,
            invoice.Amount,
            invoice.Id,
            currentTime);
        var expectedReceivedOperationId = new HistoryOperationId(_faker.Random.Long(0));

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id, senderAccount)
            .SetupQueryAccountById(receiverAccount.Id, receiverAccount)
            .SetupUpdateAccount([senderAccount, receiverAccount]);

        _persistenceContext.InvoicesRepository
            .SetupUpdateInvoice(invoice)
            .SetupQueryInvoiceById(invoice.Id, invoice);

        _persistenceContext.HistoryOperationsRepository
            .SetupAddHistoryOperation(
                [operationSenderAccount, operationReceiverAccount],
                [expectedIssuedOperationId, expectedReceivedOperationId]);

        var invoiceDto = new InvoiceDto(
            invoice.Id.Value,
            invoice.SenderAccountId.Value,
            invoice.ReceiverAccountId.Value,
            invoice.Amount.Value,
            InvoiceStatusDto.Paid,
            invoice.CreatedAt,
            invoice.UpdatedAt);

        _serviceMetrics.Setup(m => m.IncInvoicePaid());

        // Act
        PayInvoice.Response response = await _invoiceService.PayAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<PayInvoice.Response.Success>()
            .Which.Invoice.Should()
            .BeEquivalentTo(invoiceDto);
    }

    [Fact]
    public async Task PayAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<Account>().Generate();
        var invoiceAmount = new Money(_faker.Finance.Amount());
        Account receiverAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .RuleFor(a => a.Balance, invoiceAmount)
            .Generate()
            .MapToDomain();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .RuleFor(i => i.Amount, invoiceAmount)
            .Generate()
            .MapToDomain();

        PayInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId);

        // Act
        PayInvoice.Response response = await _invoiceService.PayAsync(request, default);

        // Assert
        response.Should().BeOfType<PayInvoice.Response.Unauthorized>();
    }

    [Fact]
    public async Task PayAsync_ShouldReturnInvoiceNotFound_WhenInvoiceNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        var invoiceId = new InvoiceId(_faker.Random.Long(0));

        PayInvoice.Request request = new(user.AuthorizationId, invoiceId.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoiceId);

        // Act
        PayInvoice.Response response = await _invoiceService.PayAsync(request, default);

        // Assert
        response.Should().BeOfType<PayInvoice.Response.InvoiceNotFound>();
    }

    [Theory]
    [MemberData(nameof(GetTerminalStates))]
    public async Task PayAsync_ShouldReturnInvalidInvoiceState_WhenInvoiceCantGoToPaidState(
        IInvoiceState invoiceState)
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<Account>().Generate();
        var invoiceAmount = new Money(_faker.Finance.Amount());
        Account receiverAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .RuleFor(a => a.Balance, invoiceAmount)
            .Generate()
            .MapToDomain();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, invoiceState)
            .RuleFor(i => i.Amount, invoiceAmount)
            .Generate()
            .MapToDomain();

        PayInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        PayInvoice.Response response = await _invoiceService.PayAsync(request, default);

        // Assert
        response.Should().BeOfType<PayInvoice.Response.InvalidInvoiceState>();
    }

    [Fact]
    public async Task PayAsync_ShouldReturnAccountNotFound_WhenSenderAccountNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<Account>().Generate();
        var invoiceAmount = new Money(_faker.Finance.Amount());
        Account receiverAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .RuleFor(a => a.Balance, invoiceAmount)
            .Generate()
            .MapToDomain();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .RuleFor(i => i.Amount, invoiceAmount)
            .Generate()
            .MapToDomain();

        PayInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        PayInvoice.Response response = await _invoiceService.PayAsync(request, default);

        // Assert
        response.Should().BeOfType<PayInvoice.Response.AccountNotFound>();
    }

    [Fact]
    public async Task PayAsync_ShouldReturnAccountNotFound_WhenReceiverAccountNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<Account>().Generate();
        var invoiceAmount = new Money(_faker.Finance.Amount());
        Account receiverAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .RuleFor(a => a.Balance, invoiceAmount)
            .Generate()
            .MapToDomain();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .RuleFor(i => i.Amount, invoiceAmount)
            .Generate()
            .MapToDomain();

        PayInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id, senderAccount)
            .SetupQueryAccountById(receiverAccount.Id);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        PayInvoice.Response response = await _invoiceService.PayAsync(request, default);

        // Assert
        response.Should().BeOfType<PayInvoice.Response.AccountNotFound>();
    }

    [Fact]
    public async Task PayAsync_ShouldReturnForbidden_WhenReceiverAccountNotUsers()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>().Generate().MapToDomain();
        var invoiceAmount = new Money(_faker.Finance.Amount());
        Account receiverAccount = new AutoFaker<Account>()
            .RuleFor(a => a.Balance, invoiceAmount)
            .Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .RuleFor(i => i.Amount, invoiceAmount)
            .Generate()
            .MapToDomain();

        PayInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id, senderAccount)
            .SetupQueryAccountById(receiverAccount.Id, receiverAccount);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        PayInvoice.Response response = await _invoiceService.PayAsync(request, default);

        // Assert
        response.Should().BeOfType<PayInvoice.Response.Forbidden>();
    }

    [Fact]
    public async Task PayAsync_ShouldReturnWithdrawalError_WhenReceiverHasntMoney()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<Account>().Generate();
        var invoiceAmount = new Money(_faker.Finance.Amount());
        Account receiverAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .RuleFor(i => i.Amount, invoiceAmount)
            .Generate()
            .MapToDomain();

        PayInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        _dateTimeProvider.Setup(time => time.Current).Returns(_faker.Date.RecentOffset());

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id, senderAccount)
            .SetupQueryAccountById(receiverAccount.Id, receiverAccount);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        PayInvoice.Response response = await _invoiceService.PayAsync(request, default);

        // Assert
        response.Should().BeOfType<PayInvoice.Response.WithdrawalError>();
    }

    [Fact]
    public async Task RevokeAsync_ShouldReturnDto()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();
        Account receiverAccount = new AutoFaker<Account>().Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .Generate()
            .MapToDomain();

        RevokeInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        DateTimeOffset currentTime = _faker.Date.RecentOffset();
        _dateTimeProvider.Setup(time => time.Current).Returns(currentTime);

        var operationSenderAccount = new InvoiceRevokedHistoryOperation(
            HistoryOperationId.Default,
            senderAccount.Id,
            invoice.Id,
            currentTime);
        var expectedIssuedOperationId = new HistoryOperationId(_faker.Random.Long(0));

        var operationReceiverAccount = new InvoiceRevokedHistoryOperation(
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
            .SetupUpdateInvoice(invoice)
            .SetupQueryInvoiceById(invoice.Id, invoice);

        _persistenceContext.HistoryOperationsRepository
            .SetupAddHistoryOperation(
                [operationSenderAccount, operationReceiverAccount],
                [expectedIssuedOperationId, expectedReceivedOperationId]);

        var invoiceDto = new InvoiceDto(
            invoice.Id.Value,
            invoice.SenderAccountId.Value,
            invoice.ReceiverAccountId.Value,
            invoice.Amount.Value,
            InvoiceStatusDto.Revoked,
            invoice.CreatedAt,
            invoice.UpdatedAt);

        _serviceMetrics.Setup(m => m.IncInvoiceRevoked());

        // Act
        RevokeInvoice.Response response = await _invoiceService.RevokeAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<RevokeInvoice.Response.Success>()
            .Which.Invoice.Should()
            .BeEquivalentTo(invoiceDto);
    }

    [Fact]
    public async Task RevokeAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();
        Account receiverAccount = new AutoFaker<Account>().Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .Generate()
            .MapToDomain();

        RevokeInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId);

        // Act
        RevokeInvoice.Response response = await _invoiceService.RevokeAsync(request, default);

        // Assert
        response.Should().BeOfType<RevokeInvoice.Response.Unauthorized>();
    }

    [Fact]
    public async Task RevokeAsync_ShouldReturnInvoiceNotFound_WhenInvoiceNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();
        Account receiverAccount = new AutoFaker<Account>().Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .Generate()
            .MapToDomain();

        RevokeInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id);

        // Act
        RevokeInvoice.Response response = await _invoiceService.RevokeAsync(request, default);

        // Assert
        response.Should().BeOfType<RevokeInvoice.Response.InvoiceNotFound>();
    }

    [Theory]
    [MemberData(nameof(GetTerminalStates))]
    public async Task RevokeAsync_ShouldReturnInvalidInvoiceState_WhenInvoiceCantGoToRevokedState(
        IInvoiceState invoiceState)
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();
        Account receiverAccount = new AutoFaker<Account>().Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, invoiceState)
            .Generate()
            .MapToDomain();

        RevokeInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        RevokeInvoice.Response response = await _invoiceService.RevokeAsync(request, default);

        // Assert
        response.Should().BeOfType<RevokeInvoice.Response.InvalidInvoiceState>();
    }

    [Fact]
    public async Task RevokeAsync_ShouldReturnAccountNotFound_WhenSenserAccountNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();
        Account receiverAccount = new AutoFaker<Account>().Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .Generate()
            .MapToDomain();

        RevokeInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        RevokeInvoice.Response response = await _invoiceService.RevokeAsync(request, default);

        // Assert
        response.Should().BeOfType<RevokeInvoice.Response.AccountNotFound>();
    }

    [Fact]
    public async Task RevokeAsync_ShouldReturnAccountNotFound_WhenReceiverAccountNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();
        Account receiverAccount = new AutoFaker<Account>().Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .Generate()
            .MapToDomain();

        RevokeInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id, senderAccount)
            .SetupQueryAccountById(receiverAccount.Id);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        RevokeInvoice.Response response = await _invoiceService.RevokeAsync(request, default);

        // Assert
        response.Should().BeOfType<RevokeInvoice.Response.AccountNotFound>();
    }

    [Fact]
    public async Task RevokeAsync_ShouldReturnForbidden_WhenSenderAccountNotUsers()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account senderAccount = new AutoFaker<AccountTestModel>()
            .Generate()
            .MapToDomain();
        Account receiverAccount = new AutoFaker<Account>().Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .Generate()
            .MapToDomain();

        RevokeInvoice.Request request = new(user.AuthorizationId, invoice.Id.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id, senderAccount);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        RevokeInvoice.Response response = await _invoiceService.RevokeAsync(request, default);

        // Assert
        response.Should().BeOfType<RevokeInvoice.Response.Forbidden>();
    }

    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(2, 1, false)]
    [InlineData(1, 2, true)]
    public async Task GetAsync_ShouldReturnDtos(
        int pageSize,
        int invoiceCount,
        bool pageTokenReturned)
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account accounts = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();
        var invoices = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .Generate(invoiceCount)
            .Select(model => model.MapToDomain())
            .ToList();

        GetInvoices.Request request = new(user.AuthorizationId, [], [], InvoiceTypeDto.Incoming, pageSize, null);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountByUserId(user.Id, accounts);

        _persistenceContext.InvoicesRepository
            .Setup(repo => repo.QueryAsync(
                It.Is<InvoiceQuery>(q => q.PageSize == pageSize),
                It.IsAny<CancellationToken>()))
            .Returns(invoices.ToAsyncEnumerable());

        // Act
        GetInvoices.Response response = await _invoiceService.GetAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<GetInvoices.Response.Success>()
            .Which.Invoices.Should()
            .HaveCount(invoiceCount);

        if (pageTokenReturned)
        {
            response
                .Should()
                .BeOfType<GetInvoices.Response.Success>()
                .Which.PageToken.Should()
                .NotBeNull();
        }
        else
        {
            response
                .Should()
                .BeOfType<GetInvoices.Response.Success>()
                .Which.PageToken.Should()
                .BeNull();
        }
    }

    [Fact]
    public async Task GetAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();

        GetInvoices.Request request = new(
            user.AuthorizationId,
            [],
            [],
            InvoiceTypeDto.Incoming,
            _faker.Random.Int(0),
            null);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId);

        // Act
        GetInvoices.Response response = await _invoiceService.GetAsync(request, default);

        // Assert
        response.Should().BeOfType<GetInvoices.Response.Unauthorized>();
    }
}