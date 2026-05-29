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
    public static TheoryData<IInvoiceState> GetInvalidApproveStates() => new()
    {
        new PaidInvoiceState(),
        new RevokedInvoiceState(),
        new DeclinedInvoiceState(),
        new ApprovedInvoiceState(),
    };

    [Fact]
    public async Task ApproveAsync_ShouldReturnDto()
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

        ApproveInvoice.Request request = new(invoice.Id.Value);

        DateTimeOffset currentTime = _faker.Date.RecentOffset();
        _dateTimeProvider.Setup(time => time.Current).Returns(currentTime);

        var operationSenderAccount = new InvoiceApprovedHistoryOperation(
            HistoryOperationId.Default,
            senderAccount.Id,
            invoice.Id,
            currentTime);
        var expectedIssuedOperationId = new HistoryOperationId(_faker.Random.Long(0));

        var operationReceiverAccount = new InvoiceApprovedHistoryOperation(
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
            InvoiceStatusDto.Approved,
            invoice.CreatedAt,
            invoice.UpdatedAt);

        _serviceMetrics.Setup(m => m.IncInvoiceApproved());

        // Act
        ApproveInvoice.Response response = await _invoiceService.ApproveAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<ApproveInvoice.Response.Success>()
            .Which.Invoice.Should()
            .BeEquivalentTo(invoiceDto);
    }

    [Fact]
    public async Task ApproveAsync_ShouldInvoiceNotFound_WhenInvoiceNotExist()
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

        ApproveInvoice.Request request = new(invoice.Id.Value);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id);

        // Act
        ApproveInvoice.Response response = await _invoiceService.ApproveAsync(request, default);

        // Assert
        response.Should().BeOfType<ApproveInvoice.Response.InvoiceNotFound>();
    }

    [Theory]
    [MemberData(nameof(GetInvalidApproveStates))]
    public async Task ApproveAsync_ShouldReturnInvalidInvoiceState_WhenInvoiceCantApproved(IInvoiceState invoiceState)
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

        ApproveInvoice.Request request = new(invoice.Id.Value);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        ApproveInvoice.Response response = await _invoiceService.ApproveAsync(request, default);

        // Assert
        response.Should().BeOfType<ApproveInvoice.Response.InvalidInvoiceState>();
    }

    [Fact]
    public async Task ApproveAsync_ShouldReturnAccountNotFound_WhenSenderAccountNotExist()
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

        ApproveInvoice.Request request = new(invoice.Id.Value);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        ApproveInvoice.Response response = await _invoiceService.ApproveAsync(request, default);

        // Assert
        response.Should().BeOfType<ApproveInvoice.Response.AccountNotFound>();
    }

    [Fact]
    public async Task ApproveAsync_ShouldReturnAccountNotFound_WhenReceiverAccountNotExist()
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

        ApproveInvoice.Request request = new(invoice.Id.Value);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(senderAccount.Id, senderAccount)
            .SetupQueryAccountById(receiverAccount.Id);

        _persistenceContext.InvoicesRepository
            .SetupQueryInvoiceById(invoice.Id, invoice);

        // Act
        ApproveInvoice.Response response = await _invoiceService.ApproveAsync(request, default);

        // Assert
        response.Should().BeOfType<ApproveInvoice.Response.AccountNotFound>();
    }
}