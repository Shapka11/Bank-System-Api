using Bsa.Application.Abstractions.Persistence;
using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Application.Contracts.Users;
using Bsa.Application.Contracts.Users.Operations;
using Bsa.Application.Mapping;
using Bsa.Application.Providers;
using Bsa.Domain.Accounts;
using Bsa.Domain.Accounts.Results;
using Bsa.Domain.Operations;
using Bsa.Domain.Sessions;
using Bsa.Domain.ValueObjects;
using System.Transactions;

namespace Bsa.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IPersistenceContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserService(IPersistenceContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<LoginUser.Response> LoginAsync(LoginUser.Request request, CancellationToken cancellationToken)
    {
        var accountNumber = new AccountNumber(request.AccountNumber);
        var password = new Password(request.Password);

        Account? account = await _context.AccountsRepository
            .FindAccountByNumberAsync(accountNumber, cancellationToken);

        if (account is null)
            return new LoginUser.Response.Failure("Account not found");

        if (account.VerifyPassword(password) is false)
            return new LoginUser.Response.Failure("Wrong password");

        var session = new UserSession(Guid.NewGuid(), account.Id, _dateTimeProvider.Current);
        await _context.UserSessionsRepository.AddAsync([session], cancellationToken);

        return new LoginUser.Response.Success(session.MapToDto());
    }

    public async Task<LogoutUser.Response> LogoutAsync(LogoutUser.Request request, CancellationToken cancellationToken)
    {
        UserSession? session = await _context.UserSessionsRepository
            .FindUserSessionByIdAsync(request.Id, cancellationToken);

        if (session is null)
            return new LogoutUser.Response.Failure("Session not found");

        await _context.UserSessionsRepository.RemoveAsync([session], cancellationToken);

        return new LogoutUser.Response.Success();
    }

    public async Task<DepositUserAccount.Response> DepositAsync(
        DepositUserAccount.Request request,
        CancellationToken cancellationToken)
    {
        UserSession? session = await _context.UserSessionsRepository
            .FindUserSessionByIdAsync(request.Id, cancellationToken);

        if (session is null)
            return new DepositUserAccount.Response.Failure("Session not found");

        Account? account = await _context.AccountsRepository
            .FindAccountByIdAsync(session.AccountId, cancellationToken);

        if (account is null)
            return new DepositUserAccount.Response.Failure("Account not found");

        account.Deposit(new Money(request.Amount));
        account.UpdateTime();
        var operation = new AccountOperation(
            AccountOperationId.Default,
            account.Id,
            account.Number,
            account.Balance,
            AccountOperationType.Deposit,
            _dateTimeProvider.Current);

        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _context.AccountsRepository.UpdateAsync([account], cancellationToken);

        await _context.AccountOperationsRepository
            .AddAsync([operation], cancellationToken)
            .FirstAsync(cancellationToken);

        transaction.Complete();
        return new DepositUserAccount.Response.Success(account.MapToDto());
    }

    public async Task<WithdrawUserAccount.Response> WithdrawAsync(
        WithdrawUserAccount.Request request,
        CancellationToken cancellationToken)
    {
        UserSession? session = await _context.UserSessionsRepository
            .FindUserSessionByIdAsync(request.Id, cancellationToken);

        if (session is null)
            return new WithdrawUserAccount.Response.Failure("Session not found");

        Account? account = await _context.AccountsRepository
            .FindAccountByIdAsync(session.AccountId, cancellationToken);

        if (account is null)
            return new WithdrawUserAccount.Response.Failure("Account not found");

        WithdrawResult result = account.Withdraw(new Money(request.Amount));
        if (result is WithdrawResult.Failure failure)
            return new WithdrawUserAccount.Response.Failure(failure.Error);

        account.UpdateTime();
        var operation = new AccountOperation(
            AccountOperationId.Default,
            account.Id,
            account.Number,
            account.Balance,
            AccountOperationType.Withdraw,
            _dateTimeProvider.Current);

        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _context.AccountsRepository.UpdateAsync([account], cancellationToken);

        await _context.AccountOperationsRepository
            .AddAsync([operation], cancellationToken)
            .FirstAsync(cancellationToken);

        transaction.Complete();
        return new WithdrawUserAccount.Response.Success(account.MapToDto());
    }

    public async Task<GetUserBalance.Response> GetBalanceAsync(
        GetUserBalance.Request request,
        CancellationToken cancellationToken)
    {
        UserSession? session = await _context.UserSessionsRepository
            .FindUserSessionByIdAsync(request.Id, cancellationToken);

        if (session is null)
            return new GetUserBalance.Response.Failure("Session not found");

        Account? account = await _context.AccountsRepository
            .FindAccountByIdAsync(session.AccountId, cancellationToken);

        if (account is null)
            return new GetUserBalance.Response.Failure("Account not found");

        var balance = new Money(account.Balance.Value);

        var operation = new AccountOperation(
            AccountOperationId.Default,
            account.Id,
            account.Number,
            account.Balance,
            AccountOperationType.CheckBalance,
            _dateTimeProvider.Current);

        await _context.AccountOperationsRepository
            .AddAsync([operation], cancellationToken)
            .FirstAsync(cancellationToken);

        return new GetUserBalance.Response.Success(balance.Value);
    }

    public async Task<GetUserOperationHistory.Response> GetHistoryAsync(
        GetUserOperationHistory.Request request,
        CancellationToken cancellationToken)
    {
        UserSession? session = await _context.UserSessionsRepository
            .FindUserSessionByIdAsync(request.Id, cancellationToken);

        if (session is null)
            return new GetUserOperationHistory.Response.Failure("Session not found");

        AccountId? accountIdCursor = request.PageToken?.Id is not null
            ? new AccountId(request.PageToken.Value.Id)
            : null;

        var query = AccountOperationQuery.Build(builder => builder
            .WithAccountId(session.AccountId)
            .WithIdCursor(accountIdCursor)
            .WithPageSize(request.PageSize));

        AccountOperation[] history = await _context.AccountOperationsRepository
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        GetUserOperationHistory.PageToken? responsePageToken = history.Length < request.PageSize
            ? null
            : new GetUserOperationHistory.PageToken(history[^1].Id.Value);

        return new GetUserOperationHistory.Response.Success(history.MapToDto(), responsePageToken);
    }
}