using Bsa.Application.Abstractions.Persistence;
using Bsa.Application.Contracts.Accounts;
using Bsa.Application.Contracts.Accounts.Operations;
using Bsa.Application.Mapping;
using Bsa.Domain.Accounts;
using Bsa.Domain.Accounts.Results;
using Bsa.Domain.HistoryOperations;
using Bsa.Domain.HistoryOperations.Accounts;
using Bsa.Domain.Sessions;
using Bsa.Domain.ValueObjects;
using Itmo.Dev.Platform.Common.DateTime;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using IsolationLevel = System.Data.IsolationLevel;

namespace Bsa.Application.Services;

public sealed class AccountService : IAccountService
{
    private readonly IPersistenceContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPersistenceTransactionProvider _transactionProvider;

    public AccountService(
        IPersistenceContext context,
        IDateTimeProvider dateTimeProvider,
        IPersistenceTransactionProvider transactionProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _transactionProvider = transactionProvider;
    }

    public async Task<CreateAccount.Response> CreateAsync(
        CreateAccount.Request request,
        CancellationToken cancellationToken)
    {
        SessionBase? adminSession = await _context.SessionRepository
            .FindSessionByIdAsync(request.Id, cancellationToken);

        if (adminSession is not AdminSession)
            return new CreateAccount.Response.Unauthorized(request.Id, "Session not admin");

        Account? dbAccount = await _context.AccountsRepository
            .FindAccountByNumberAsync(new AccountNumber(request.AccountNumber), cancellationToken);

        if (dbAccount is not null)
            return new CreateAccount.Response.AccountAlreadyExists(dbAccount.Number.Value);

        var account = new Account(
            AccountId.Default,
            new AccountNumber(request.AccountNumber),
            new Password(request.Password),
            Money.Zero,
            _dateTimeProvider.Current,
            _dateTimeProvider.Current);

        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        account = await _context.AccountsRepository
            .AddAsync([account], cancellationToken)
            .FirstAsync(cancellationToken);

        var operation = new CreateAccountHistoryOperation(
            HistoryOperationId.Default,
            account.Id,
            account.Number,
            _dateTimeProvider.Current);

        await _context.HistoryOperationsRepository
            .AddAsync([operation], cancellationToken)
            .FirstAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new CreateAccount.Response.Success(account.MapToDto());
    }

    public async Task<Deposit.Response> DepositAsync(
        Deposit.Request request,
        CancellationToken cancellationToken)
    {
        SessionBase? session = await _context.SessionRepository
            .FindSessionByIdAsync(request.Id, cancellationToken);

        if (session is not UserSession userSession)
            return new Deposit.Response.Unauthorized(request.Id, "Session not user");

        Account? account = await _context.AccountsRepository
            .FindAccountByIdAsync(userSession.AccountId, cancellationToken);

        if (account is null)
            return new Deposit.Response.AccountNotFound(userSession.AccountId.Value);

        var depositTotal = new Money(request.Amount);
        account.Deposit(depositTotal);
        account.UpdateTime(_dateTimeProvider.Current);
        var operation = new DepositHistoryOperation(
            HistoryOperationId.Default,
            account.Id,
            account.Number,
            depositTotal,
            _dateTimeProvider.Current);

        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        await _context.AccountsRepository.UpdateAsync([account], cancellationToken);

        await _context.HistoryOperationsRepository
            .AddAsync([operation], cancellationToken)
            .FirstAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new Deposit.Response.Success(account.MapToDto());
    }

    public async Task<Withdraw.Response> WithdrawAsync(
        Withdraw.Request request,
        CancellationToken cancellationToken)
    {
        SessionBase? session = await _context.SessionRepository
            .FindSessionByIdAsync(request.Id, cancellationToken);

        if (session is not UserSession userSession)
            return new Withdraw.Response.Unauthorized(request.Id, "Session not user");

        Account? account = await _context.AccountsRepository
            .FindAccountByIdAsync(userSession.AccountId, cancellationToken);

        if (account is null)
            return new Withdraw.Response.AccountNotFound(userSession.AccountId.Value);

        var withdrawTotal = new Money(request.Amount);
        WithdrawResult result = account.Withdraw(withdrawTotal);
        if (result is WithdrawResult.Failure failure)
            return new Withdraw.Response.InsufficientFunds(failure.ErrorMessage);

        account.UpdateTime(_dateTimeProvider.Current);
        var operation = new WithdrawHistoryOperation(
            HistoryOperationId.Default,
            account.Id,
            account.Number,
            withdrawTotal,
            _dateTimeProvider.Current);

        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        await _context.AccountsRepository.UpdateAsync([account], cancellationToken);

        await _context.HistoryOperationsRepository
            .AddAsync([operation], cancellationToken)
            .FirstAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new Withdraw.Response.Success(account.MapToDto());
    }

    public async Task<GetBalance.Response> GetBalanceAsync(
        GetBalance.Request request,
        CancellationToken cancellationToken)
    {
        SessionBase? session = await _context.SessionRepository
            .FindSessionByIdAsync(request.Id, cancellationToken);

        if (session is not UserSession userSession)
            return new GetBalance.Response.Unauthorized(request.Id, "Session not user");

        Account? account = await _context.AccountsRepository
            .FindAccountByIdAsync(userSession.AccountId, cancellationToken);

        if (account is null)
            return new GetBalance.Response.AccountNotFound(userSession.AccountId.Value);

        var balance = new Money(account.Balance.Value);

        var operation = new CheckBalanceHistoryOperation(
            HistoryOperationId.Default,
            account.Id,
            account.Number,
            account.Balance,
            _dateTimeProvider.Current);

        await _context.HistoryOperationsRepository
            .AddAsync([operation], cancellationToken)
            .FirstAsync(cancellationToken);

        return new GetBalance.Response.Success(balance.Value);
    }
}