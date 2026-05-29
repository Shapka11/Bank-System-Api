using AutoBogus;
using BankSystemApi.Application.Contracts.Invoices.Models;
using BankSystemApi.Application.Contracts.Invoices.Operations;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Invoices;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Domain.Invoices.States;
using FluentAssertions;
using UnitTests.MockExtensions;
using UnitTests.Models;

namespace UnitTests.ServiceTests.Invoices;

public sealed partial class InvoiceServiceTests
{
    public static TheoryData<IInvoiceState> GetInvalidDeclineStates() => new()
    {
        new PaidInvoiceState(),
        new RevokedInvoiceState(),
        new DeclinedInvoiceState(),
        new ApprovedInvoiceState(),
    };

    [Fact]
    public async Task DeclineAsync_ShouldReturnDto()
    {
        // Arrange
        Account senderAccount = new AutoFaker<Account>().Generate();
        Account receiverAccount = new AutoFaker<Account>().Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .Generate()
            .MapToDomain();

        DeclineInvoice.Request request = new(invoice.Id.Value);

        DateTimeOffset currentTime = _faker.Date.RecentOffset();
        _dateTimeProvider.Setup(time => time.Current).Returns(currentTime);

        var operationSenderAccount = new InvoiceDeclinedHistoryOperation(
            HistoryOperationId.Default,
            senderAccount.Id,
            invoice.Id,
            currentTime);
        var expectedIssuedOperationId = new HistoryOperationId(_faker.Random.Long(0));

        var operationReceiverAccount = new InvoiceDeclinedHistoryOperation(
            HistoryOperationId.Default,
            receiverAccount.Id,
            invoice.Id,
            currentTime);
        var expectedReceivedOperationId = new HistoryOperationId(_faker.Random.Long(0));

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
            InvoiceStatusDto.Declined,
            invoice.CreatedAt,
            invoice.UpdatedAt);

        _serviceMetrics.Setup(m => m.InvInvoiceDeclined());

        // Act
        DeclineInvoice.Response response = await _invoiceService.DeclineAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<DeclineInvoice.Response.Success>()
            .Which.Invoice.Should()
            .BeEquivalentTo(invoiceDto);
    }

    [Fact]
    public async Task DeclineAsync_ShouldInvoiceNotFound_WhenInvoiceNotExist()
    {
        // Arrange
        Account senderAccount = new AutoFaker<Account>().Generate();
        Account receiverAccount = new AutoFaker<Account>().Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .Generate()
            .MapToDomain();

        DeclineInvoice.Request request = new(invoice.Id.Value);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id);

        // Act
        DeclineInvoice.Response response = await _invoiceService.DeclineAsync(request, default);

        // Assert
        response.Should().BeOfType<DeclineInvoice.Response.InvoiceNotFound>();
    }

    [Theory]
    [MemberData(nameof(GetInvalidDeclineStates))]
    public async Task DeclineAsync_ShouldReturnInvalidInvoiceState_WhenInvoiceCantDeclined(IInvoiceState invoiceState)
    {
        // Arrange
        Account senderAccount = new AutoFaker<Account>().Generate();
        Account receiverAccount = new AutoFaker<Account>().Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, invoiceState)
            .Generate()
            .MapToDomain();

        DeclineInvoice.Request request = new(invoice.Id.Value);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        DeclineInvoice.Response response = await _invoiceService.DeclineAsync(request, default);

        // Assert
        response.Should().BeOfType<DeclineInvoice.Response.InvalidInvoiceState>();
    }

    [Fact]
    public async Task DeclineAsync_ShouldReturnAccountNotFound_WhenSenderAccountNotExist()
    {
        // Arrange
        Account senderAccount = new AutoFaker<Account>().Generate();
        Account receiverAccount = new AutoFaker<Account>().Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .Generate()
            .MapToDomain();

        DeclineInvoice.Request request = new(invoice.Id.Value);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        DeclineInvoice.Response response = await _invoiceService.DeclineAsync(request, default);

        // Assert
        response.Should().BeOfType<DeclineInvoice.Response.AccountNotFound>();
    }

    [Fact]
    public async Task DeclineAsync_ShouldReturnAccountNotFound_WhenReceiverAccountNotExist()
    {
        // Arrange
        Account senderAccount = new AutoFaker<Account>().Generate();
        Account receiverAccount = new AutoFaker<Account>().Generate();
        Invoice invoice = new AutoFaker<InvoiceTestModel>()
            .RuleFor(i => i.SenderAccountId, senderAccount.Id)
            .RuleFor(i => i.ReceiverAccountId, receiverAccount.Id)
            .RuleFor(i => i.State, new CreatedInvoiceState())
            .Generate()
            .MapToDomain();

        DeclineInvoice.Request request = new(invoice.Id.Value);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id, senderAccount)
            .SetupQueryAccountById(receiverAccount.Id);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        DeclineInvoice.Response response = await _invoiceService.DeclineAsync(request, default);

        // Assert
        response.Should().BeOfType<DeclineInvoice.Response.AccountNotFound>();
    }
}