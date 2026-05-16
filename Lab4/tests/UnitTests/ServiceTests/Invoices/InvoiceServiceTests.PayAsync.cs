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
}