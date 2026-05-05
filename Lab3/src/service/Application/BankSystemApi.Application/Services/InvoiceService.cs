using BankSystemApi.Application.Abstractions.Metrics;
using BankSystemApi.Application.Abstractions.Persistence;
using BankSystemApi.Application.Activities;
using BankSystemApi.Application.Contracts.Invoices;
using BankSystemApi.Application.Contracts.Invoices.Models;
using BankSystemApi.Application.Contracts.Invoices.Operations;
using BankSystemApi.Application.Mapping;
using BankSystemApi.Application.Specifications;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Accounts.Results;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Invoices;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Domain.Invoices.Results;
using BankSystemApi.Domain.Invoices.States;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using Itmo.Dev.Platform.Common.DateTime;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;
using InvoiceQuery = BankSystemApi.Application.Abstractions.Persistence.Queries.InvoiceQuery;

namespace BankSystemApi.Application.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IPersistenceContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPersistenceTransactionProvider _transactionProvider;
    private readonly ILogger<InvoiceService> _logger;
    private readonly IServiceMetrics _metrics;

    public InvoiceService(
        IPersistenceContext context,
        IDateTimeProvider dateTimeProvider,
        IPersistenceTransactionProvider transactionProvider,
        ILogger<InvoiceService> logger,
        IServiceMetrics metrics)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _transactionProvider = transactionProvider;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<CreateInvoice.Response> CreateAsync(
        CreateInvoice.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = InvoiceServiceActivity.ActivitySource.StartActivity();
        activity?.SetTag("account.id", request.SenderAccountId);
        activity?.SetTag("user.id", request.UserId);

        User? user = await _context.UserRepository.FindByAuthorizationIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Unauthorized access attempt: User ID '{UserId}' is not exist.", request.UserId);
            return new CreateInvoice.Response.Unauthorized(request.UserId);
        }

        var invoice = new Invoice(
            InvoiceId.Default,
            new AccountId(request.SenderAccountId),
            new AccountId(request.ReceiverAccountId),
            new Money(request.Amount),
            new CreatedInvoiceState(),
            _dateTimeProvider.Current,
            _dateTimeProvider.Current);

        Account? senderAccount = await _context.AccountsRepository
            .FindAccountByIdAsync(invoice.SenderAccountId, cancellationToken);
        if (senderAccount is null)
        {
            _logger.LogWarning("Account with id {AccountId} not found.", invoice.SenderAccountId.Value);
            return new CreateInvoice.Response.SenderAccountNotFound(invoice.SenderAccountId.Value);
        }

        if (user.Id != senderAccount.UserId)
        {
            _logger.LogWarning(
                "Account {AccountId} does to belong to the user {UserId}",
                senderAccount.Id.Value,
                user.Id.Value);
            return new CreateInvoice.Response.Forbidden("The sender's account is not yours");
        }

        Account? receiverAccount = await _context.AccountsRepository
            .FindAccountByIdAsync(invoice.ReceiverAccountId, cancellationToken);
        if (receiverAccount is null)
        {
            _logger.LogWarning("Account with id {AccountId} not found.", invoice.ReceiverAccountId.Value);
            return new CreateInvoice.Response.ReceiverAccountNotFound(invoice.ReceiverAccountId.Value);
        }

        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        invoice = await _context.InvoiceRepository
            .AddAsync([invoice], cancellationToken)
            .FirstAsync(cancellationToken);

        var operationSenderAccount = new InvoiceIssuedHistoryOperation(
            HistoryOperationId.Default,
            senderAccount.Id,
            invoice.Id,
            _dateTimeProvider.Current);

        var operationReceiverAccount = new InvoiceReceivedHistoryOperation(
            HistoryOperationId.Default,
            receiverAccount.Id,
            invoice.Id,
            _dateTimeProvider.Current);

        await _context.HistoryOperationsRepository
            .AddAsync([operationSenderAccount, operationReceiverAccount], cancellationToken)
            .FirstAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation("Invoice {InvoiceId} created successfully", invoice.Id.Value);

        _metrics.IncInvoiceCreated();

        return new CreateInvoice.Response.Success(invoice.MapToDto());
    }

    public async Task<PayInvoice.Response> PayAsync(
        PayInvoice.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = InvoiceServiceActivity.ActivitySource.StartActivity();
        activity?.SetTag("user.id", request.UserId);
        activity?.SetTag("invoice.id", request.InvoiceId);

        User? user = await _context.UserRepository.FindByAuthorizationIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Unauthorized access attempt: User ID '{UserId}' is not exist.", request.UserId);
            return new PayInvoice.Response.Unauthorized(request.UserId);
        }

        var invoiceId = new InvoiceId(request.InvoiceId);
        Invoice? invoice = await _context.InvoiceRepository.FindById(invoiceId, cancellationToken);

        if (invoice is null)
        {
            _logger.LogWarning("Invoice {InvoiceId} not found", invoiceId.Value);
            return new PayInvoice.Response.InvoiceNotFound(invoiceId.Value);
        }

        PayInvoiceResult invoicePayResult = invoice.Pay();
        if (invoicePayResult is PayInvoiceResult.Failure)
        {
            _logger.LogWarning("Invoice {InvoiceId} attempt pay failed", invoiceId.Value);
            return new PayInvoice.Response.InvalidInvoiceState(invoice.State.State.ToString());
        }

        Account? senderAccount = await _context.AccountsRepository
            .FindAccountByIdAsync(invoice.SenderAccountId, cancellationToken);
        if (senderAccount is null)
        {
            _logger.LogWarning("Account with id {AccountId} not found.", invoice.SenderAccountId.Value);
            return new PayInvoice.Response.AccountNotFound(invoice.SenderAccountId.Value);
        }

        if (user.Id != senderAccount.UserId)
        {
            _logger.LogWarning(
                "Account {AccountId} does to belong to the user {UserId}",
                senderAccount.Id.Value,
                user.Id.Value);
            return new PayInvoice.Response.Forbidden("The sender's account is not yours");
        }

        Account? receiverAccount = await _context.AccountsRepository
            .FindAccountByIdAsync(invoice.ReceiverAccountId, cancellationToken);
        if (receiverAccount is null)
        {
            _logger.LogWarning("Account with id {AccountId} not found.", invoice.ReceiverAccountId.Value);
            return new PayInvoice.Response.AccountNotFound(invoice.ReceiverAccountId.Value);
        }

        senderAccount.Deposit(invoice.Amount);
        var operationSenderAccount = new InvoicePaymentSentHistoryOperation(
            HistoryOperationId.Default,
            senderAccount.Id,
            invoice.Amount,
            invoice.Id,
            _dateTimeProvider.Current);

        WithdrawResult withdrawResult = receiverAccount.Withdraw(invoice.Amount);

        if (withdrawResult is WithdrawResult.Failure failure)
        {
            _logger.LogWarning("Account {AccountId} withdrawal failur", receiverAccount.Id.Value);
            return new PayInvoice.Response.WithdrawalError(receiverAccount.Id.Value, failure.ErrorMessage);
        }

        var operationReceiverAccount = new InvoicePaymentReceivedHistoryOperation(
            HistoryOperationId.Default,
            receiverAccount.Id,
            invoice.Amount,
            invoice.Id,
            _dateTimeProvider.Current);

        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        await _context.AccountsRepository.UpdateAsync([senderAccount, receiverAccount], cancellationToken);

        await _context.HistoryOperationsRepository
            .AddAsync([operationSenderAccount, operationReceiverAccount], cancellationToken)
            .FirstAsync(cancellationToken);

        await _context.InvoiceRepository.UpdateAsync([invoice], cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation("Invoice {InvoiceId} paid successfully", invoice.Id.Value);

        _metrics.IncInvoicePaid();

        return new PayInvoice.Response.Success(invoice.MapToDto());
    }

    public async Task<RevokeInvoice.Response> RevokeAsync(
        RevokeInvoice.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = InvoiceServiceActivity.ActivitySource.StartActivity();
        activity?.SetTag("user.id", request.UserId);
        activity?.SetTag("invoice.id", request.InvoiceId);

        User? user = await _context.UserRepository.FindByAuthorizationIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Unauthorized access attempt: User ID '{UserId}' is not exist.", request.UserId);
            return new RevokeInvoice.Response.Unauthorized(request.UserId);
        }

        var invoiceId = new InvoiceId(request.InvoiceId);
        Invoice? invoice = await _context.InvoiceRepository.FindById(invoiceId, cancellationToken);
        if (invoice is null)
        {
            _logger.LogWarning("Invoice {InvoiceId} not found", invoiceId.Value);
            return new RevokeInvoice.Response.InvoiceNotFound(invoiceId.Value);
        }

        RevokeInvoiceResult invoiceRevokeResult = invoice.Revoke();
        if (invoiceRevokeResult is RevokeInvoiceResult.Failure failure)
        {
            _logger.LogWarning("Invoice {InvoiceId} attempt revoke failed", invoiceId.Value);
            return new RevokeInvoice.Response.InvalidInvoiceState(failure.ErrorMessage);
        }

        Account? senderAccount = await _context.AccountsRepository
            .FindAccountByIdAsync(invoice.SenderAccountId, cancellationToken);
        if (senderAccount is null)
        {
            _logger.LogWarning("Account with id {AccountId} not found.", invoice.SenderAccountId.Value);
            return new RevokeInvoice.Response.AccountNotFound(invoice.SenderAccountId.Value);
        }

        if (user.Id != senderAccount.UserId)
        {
            _logger.LogWarning(
                "Account {AccountId} does to belong to the user {UserId}",
                senderAccount.Id.Value,
                user.Id.Value);
            return new RevokeInvoice.Response.Forbidden("The sender's account is not yours");
        }

        Account? receiverAccount = await _context.AccountsRepository
            .FindAccountByIdAsync(invoice.ReceiverAccountId, cancellationToken);
        if (receiverAccount is null)
        {
            _logger.LogWarning("Account with id {AccountId} not found.", invoice.ReceiverAccountId.Value);
            return new RevokeInvoice.Response.AccountNotFound(invoice.ReceiverAccountId.Value);
        }

        var senderAccountOperation = new InvoiceRevokedHistoryOperation(
            HistoryOperationId.Default,
            senderAccount.Id,
            invoice.Id,
            _dateTimeProvider.Current);

        var receiverAccountOperation = new InvoiceRevokedHistoryOperation(
            HistoryOperationId.Default,
            receiverAccount.Id,
            invoice.Id,
            _dateTimeProvider.Current);

        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        await _context.InvoiceRepository.UpdateAsync([invoice], cancellationToken);

        await _context.HistoryOperationsRepository
            .AddAsync([senderAccountOperation, receiverAccountOperation], cancellationToken)
            .FirstAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation("Invoice {InvoiceId} revoked successfully", invoice.Id.Value);

        _metrics.IncInvoiceRevoked();

        return new RevokeInvoice.Response.Success(invoice.MapToDto());
    }

    public async Task<GetInvoices.Response> GetAsync(
        GetInvoices.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = InvoiceServiceActivity.ActivitySource.StartActivity();
        activity?.SetTag("user.id", request.UserId);

        User? user = await _context.UserRepository.FindByAuthorizationIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Unauthorized access attempt: User ID '{UserId}' is not exist.", request.UserId);
            return new GetInvoices.Response.Unauthorized(request.UserId);
        }

        Account[] accounts = await _context.AccountsRepository.GetAllByUserId(user.Id, cancellationToken);

        InvoiceId? invoiceIdCursor = request.PageToken?.Id is not null
            ? new InvoiceId(request.PageToken.Value.Id)
            : null;

        var query = InvoiceQuery.Build(builder =>
        {
            builder.WithPageSize(request.PageSize)
                .WithInvoiceIdCursor(invoiceIdCursor)
                .WithStatuses(request.Statuses.Select(s => s.MapToDomain()));

            if (request.InvoiceType is InvoiceTypeDto.Incoming)
            {
                builder.WithReceiverAccountIds(accounts.Select(a => a.Id))
                    .WithSenderAccountIds(request.ForeignAccountIds.Select(sai => new AccountId(sai)));
            }
            else
            {
                builder.WithSenderAccountIds(accounts.Select(a => a.Id))
                    .WithReceiverAccountIds(request.ForeignAccountIds.Select(rai => new AccountId(rai)));
            }

            return builder;
        });

        Invoice[] invoices = await _context.InvoiceRepository
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        GetInvoices.PageToken? responsePageToken = invoices.Length < request.PageSize
            ? null
            : new GetInvoices.PageToken(invoices.Last().Id.Value);

        return new GetInvoices.Response.Success(
            invoices.Select(i => i.MapToDto()).ToArray(),
            responsePageToken);
    }
}