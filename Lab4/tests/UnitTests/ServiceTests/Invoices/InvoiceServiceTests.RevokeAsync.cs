using AutoBogus;
using BankSystemApi.Application.Contracts.Invoices.Models;
using BankSystemApi.Application.Contracts.Invoices.Operations;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Invoices;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Domain.Invoices.States;
using BankSystemApi.Domain.Users;
using FluentAssertions;
using UnitTests.MockExtensions;
using UnitTests.Models;

namespace UnitTests.ServiceTests.Invoices;

public sealed partial class InvoiceServiceTests
{
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
}