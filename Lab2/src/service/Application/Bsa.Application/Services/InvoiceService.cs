using Bsa.Application.Abstractions.Persistence;
using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Application.Contracts.Invoices;
using Bsa.Application.Contracts.Invoices.Operations;
using Bsa.Application.Mapping;
using Bsa.Application.Specifications;
using Bsa.Domain.Accounts;
using Bsa.Domain.Accounts.Results;
using Bsa.Domain.HistoryOperations;
using Bsa.Domain.HistoryOperations.Invoices;
using Bsa.Domain.Invoices;
using Bsa.Domain.Invoices.Results;
using Bsa.Domain.Invoices.States;
using Bsa.Domain.Sessions;
using Bsa.Domain.ValueObjects;
using Itmo.Dev.Platform.Common.DateTime;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using System.Data;

namespace Bsa.Application.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IPersistenceContext _context;
    private readonly AccountSpecifications _accountSpecifications;
    private readonly InvoiceSpecifications _invoiceSpecifications;
    private readonly SessionSpecifications _sessionSpecifications;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPersistenceTransactionProvider _transactionProvider;

    public InvoiceService(
        IPersistenceContext context,
        IDateTimeProvider dateTimeProvider,
        IPersistenceTransactionProvider transactionProvider,
        AccountSpecifications accountSpecifications,
        InvoiceSpecifications invoiceSpecifications,
        SessionSpecifications sessionSpecifications)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _transactionProvider = transactionProvider;
        _accountSpecifications = accountSpecifications;
        _invoiceSpecifications = invoiceSpecifications;
        _sessionSpecifications = sessionSpecifications;
    }

    public async Task<CreateInvoice.Response> CreateAsync(
        CreateInvoice.Request request,
        CancellationToken cancellationToken)
    {
        SessionBase? session = await _sessionSpecifications.FindSessionByIdAsync(request.SessionId, cancellationToken);
        if (session is not UserSession userSession)
            return new CreateInvoice.Response.Unauthorized(request.SessionId, "Session not user");

        var invoice = new Invoice(
            InvoiceId.Default,
            new AccountNumber(request.SenderAccountNumber),
            new AccountNumber(request.ReceiverAccountNumber),
            new Money(request.Amount),
            new CreatedInvoiceState(),
            _dateTimeProvider.Current,
            _dateTimeProvider.Current);

        Account? senderAccount = await _accountSpecifications
            .FindAccountByNumberAsync(invoice.SenderAccountNumber, cancellationToken);
        Account? receiverAccount = await _accountSpecifications
            .FindAccountByNumberAsync(invoice.ReceiverAccountNumber, cancellationToken);

        if (senderAccount is null)
            return new CreateInvoice.Response.SenderAccountNotFound(invoice.SenderAccountNumber.Value);
        if (receiverAccount is null)
            return new CreateInvoice.Response.ReceiverAccountNotFound(invoice.ReceiverAccountNumber.Value);
        if (userSession.AccountId != senderAccount.Id)
            return new CreateInvoice.Response.Forbidden("The sender's account is not yours");

        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        invoice = await _context.InvoiceRepository
            .AddAsync([invoice], cancellationToken)
            .FirstAsync(cancellationToken);

        var operationSenderAccount = new InvoiceIssuedHistoryOperation(
            HistoryOperationId.Default,
            senderAccount.Id,
            senderAccount.Number,
            invoice.Id,
            _dateTimeProvider.Current);

        var operationReceiverAccount = new InvoiceReceivedHistoryOperation(
            HistoryOperationId.Default,
            receiverAccount.Id,
            receiverAccount.Number,
            invoice.Id,
            _dateTimeProvider.Current);

        await _context.HistoryOperationsRepository
            .AddAsync([operationSenderAccount, operationReceiverAccount], cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new CreateInvoice.Response.Success(invoice.MapToDto());
    }

    public async Task<PayInvoice.Response> PayAsync(
        PayInvoice.Request request,
        CancellationToken cancellationToken)
    {
        SessionBase? session = await _sessionSpecifications.FindSessionByIdAsync(request.SessionId, cancellationToken);
        if (session is not UserSession userSession)
            return new PayInvoice.Response.Unauthorized(request.SessionId, "Session not user");

        var invoiceId = new InvoiceId(request.InvoiceId);
        Invoice? invoice = await _invoiceSpecifications.FindById(invoiceId, cancellationToken);

        if (invoice is null)
            return new PayInvoice.Response.InvoiceNotFound(invoiceId.Value);

        PayInvoiceResult invoicePayResult = invoice.Pay();
        if (invoicePayResult is PayInvoiceResult.Failure)
            return new PayInvoice.Response.InvalidInvoiceState(invoice.State.State.ToString());

        Account? senderAccount = await _accountSpecifications
            .FindAccountByNumberAsync(invoice.SenderAccountNumber, cancellationToken);
        Account? receiverAccount = await _accountSpecifications
            .FindAccountByNumberAsync(invoice.ReceiverAccountNumber, cancellationToken);
        if (receiverAccount is null)
            return new PayInvoice.Response.AccountNotFound(invoice.ReceiverAccountNumber.Value);
        if (senderAccount is null)
            return new PayInvoice.Response.AccountNotFound(invoice.SenderAccountNumber.Value);
        if (userSession.AccountId != receiverAccount.Id)
            return new PayInvoice.Response.Forbidden("The receiver's account is not yours");

        senderAccount.Deposit(invoice.Amount);
        var operationSenderAccount = new InvoicePaymentSentHistoryOperation(
            HistoryOperationId.Default,
            senderAccount.Id,
            senderAccount.Number,
            invoice.Amount,
            invoice.Id,
            _dateTimeProvider.Current);

        WithdrawResult withdrawResult = receiverAccount.Withdraw(invoice.Amount);

        if (withdrawResult is WithdrawResult.Failure failure)
            return new PayInvoice.Response.WithdrawalError(receiverAccount.Number.Value, failure.ErrorMessage);

        var operationReceiverAccount = new InvoicePaymentReceivedHistoryOperation(
            HistoryOperationId.Default,
            receiverAccount.Id,
            receiverAccount.Number,
            invoice.Amount,
            invoice.Id,
            _dateTimeProvider.Current);

        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        await _context.AccountsRepository.UpdateAsync([senderAccount, receiverAccount], cancellationToken);
        await _context.HistoryOperationsRepository
            .AddAsync([operationSenderAccount, operationReceiverAccount], cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
        await _context.InvoiceRepository.UpdateAsync([invoice], cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new PayInvoice.Response.Success(invoice.MapToDto());
    }

    public async Task<RevokeInvoice.Response> RevokeAsync(
        RevokeInvoice.Request request,
        CancellationToken cancellationToken)
    {
        SessionBase? session = await _sessionSpecifications.FindSessionByIdAsync(request.SessionId, cancellationToken);
        if (session is not UserSession userSession)
            return new RevokeInvoice.Response.Unauthorized(request.SessionId, "Session not user");

        var invoiceId = new InvoiceId(request.InvoiceId);
        Invoice? invoice = await _invoiceSpecifications.FindById(invoiceId, cancellationToken);

        if (invoice is null)
            return new RevokeInvoice.Response.InvoiceNotFound(invoiceId.Value);

        RevokeInvoiceResult invoiceRevokeResult = invoice.Revoke();
        if (invoiceRevokeResult is RevokeInvoiceResult.Failure failure)
            return new RevokeInvoice.Response.InvalidInvoiceState(failure.ErrorMessage);

        Account? senderAccount = await _accountSpecifications
            .FindAccountByNumberAsync(invoice.SenderAccountNumber, cancellationToken);
        Account? receiverAccount = await _accountSpecifications
            .FindAccountByNumberAsync(invoice.ReceiverAccountNumber, cancellationToken);
        if (receiverAccount is null)
            return new RevokeInvoice.Response.AccountNotFound(invoice.ReceiverAccountNumber.Value);
        if (senderAccount is null)
            return new RevokeInvoice.Response.AccountNotFound(invoice.SenderAccountNumber.Value);
        if (userSession.AccountId != senderAccount.Id)
            return new RevokeInvoice.Response.Forbidden("The sender's account is not yours");

        var senderAccountOperation = new InvoiceRevokedHistoryOperation(
            HistoryOperationId.Default,
            senderAccount.Id,
            senderAccount.Number,
            invoice.Id,
            _dateTimeProvider.Current);

        var receiverAccountOperation = new InvoiceRevokedHistoryOperation(
            HistoryOperationId.Default,
            receiverAccount.Id,
            receiverAccount.Number,
            invoice.Id,
            _dateTimeProvider.Current);

        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        await _context.InvoiceRepository.UpdateAsync([invoice], cancellationToken);
        await _context.HistoryOperationsRepository
            .AddAsync([senderAccountOperation, receiverAccountOperation], cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new RevokeInvoice.Response.Success(invoice.MapToDto());
    }

    public async Task<GetIncomingInvoices.Response> GetIncomingAsync(
        GetIncomingInvoices.Request request,
        CancellationToken cancellationToken)
    {
        SessionBase? session = await _sessionSpecifications.FindSessionByIdAsync(request.SessionId, cancellationToken);
        if (session is not UserSession)
            return new GetIncomingInvoices.Response.Unauthorized(request.SessionId, "Session not user");

        AccountNumber[] senderAccountNumbers =
            request.SenderAccountNumbers
                .Select(number => new AccountNumber(number))
                .ToArray();

        InvoiceId? invoiceIdCursor = request.PageToken?.Id is not null
            ? new InvoiceId(request.PageToken.Value.Id)
            : null;

        var query = InvoiceQuery.Build(builder => builder
            .WithPageSize(request.PageSize)
            .WithInvoiceIdCursor(invoiceIdCursor)
            .WithStatuses(request.Statuses.Select(s => s.MapToDomain()))
            .WithSenderAccountNumbers(senderAccountNumbers));

        Invoice[] invoices = await _context.InvoiceRepository
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        GetIncomingInvoices.PageToken? responsePageToken = invoices.Length < request.PageSize
            ? null
            : new GetIncomingInvoices.PageToken(invoices.Last().Id.Value);

        return new GetIncomingInvoices.Response.Success(
            invoices.Select(i => i.MapToDto()).ToArray(),
            responsePageToken);
    }

    public async Task<GetOutgoingInvoices.Response> GetOutgoingAsync(
        GetOutgoingInvoices.Request request,
        CancellationToken cancellationToken)
    {
        SessionBase? session = await _sessionSpecifications.FindSessionByIdAsync(request.SessionId, cancellationToken);
        if (session is not UserSession)
            return new GetOutgoingInvoices.Response.Unauthorized(request.SessionId, "Session not user");

        AccountNumber[] receiverAccountNumbers =
            request.ReceiverAccountNumbers
                .Select(number => new AccountNumber(number))
                .ToArray();

        InvoiceId? invoiceIdCursor = request.PageToken?.Id is not null
            ? new InvoiceId(request.PageToken.Value.Id)
            : null;

        var query = InvoiceQuery.Build(builder => builder
            .WithPageSize(request.PageSize)
            .WithInvoiceIdCursor(invoiceIdCursor)
            .WithStatuses(request.Statuses.Select(s => s.MapToDomain()))
            .WithReceiverAccountNumbers(receiverAccountNumbers));
        Invoice[] invoices = await _context.InvoiceRepository
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        GetOutgoingInvoices.PageToken? responsePageToken = invoices.Length < request.PageSize
            ? null
            : new GetOutgoingInvoices.PageToken(invoices.Last().Id.Value);

        return new GetOutgoingInvoices.Response.Success(
            invoices.Select(i => i.MapToDto()).ToArray(),
            responsePageToken);
    }
}