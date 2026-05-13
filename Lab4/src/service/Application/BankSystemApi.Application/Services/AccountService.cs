using BankSystemApi.Application.Abstractions.Metrics;
using BankSystemApi.Application.Abstractions.Persistence;
using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Activities;
using BankSystemApi.Application.Contracts.Accounts;
using BankSystemApi.Application.Contracts.Accounts.Operations;
using BankSystemApi.Application.Mapping;
using BankSystemApi.Application.Options;
using BankSystemApi.Application.Providers;
using BankSystemApi.Application.Specifications;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Accounts.Results;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using Itmo.Dev.Platform.Common.DateTime;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using IsolationLevel = System.Data.IsolationLevel;

namespace BankSystemApi.Application.Services;

internal sealed class AccountService : IAccountService
{
    private readonly IPersistenceContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IGuidProvider _guidProvider;
    private readonly IPersistenceTransactionProvider _transactionProvider;
    private readonly IOptionsMonitor<AccountOptions> _accountOptions;
    private readonly ILogger<AccountService> _logger;
    private readonly IServiceMetrics _metrics;

    public AccountService(
        IPersistenceContext context,
        IDateTimeProvider dateTimeProvider,
        IGuidProvider guidProvider,
        IPersistenceTransactionProvider transactionProvider,
        IOptionsMonitor<AccountOptions> accountOptions,
        ILogger<AccountService> logger,
        IServiceMetrics metrics)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _guidProvider = guidProvider;
        _transactionProvider = transactionProvider;
        _accountOptions = accountOptions;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<CreateAccount.Response> CreateAsync(
        CreateAccount.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = AccountServiceActivity.ActivitySource.StartActivity();
        activity?.SetTag("user.id", request.TargetUserId);

        var accountNumber = new AccountNumber(request.AccountNumber);
        var accountPassword = new Password(request.Password);

        User? user = await _context.UsersRepository.FindByAuthorizationIdAsync(request.CallerUserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Unauthorized access attempt: User ID '{UserId}' is not exist.", request.CallerUserId);
            return new CreateAccount.Response.Unauthorized(request.CallerUserId.ToString());
        }

        User? targetUser = await _context.UsersRepository
            .FindByIdAsync(new UserId(request.TargetUserId), cancellationToken);
        if (targetUser is null)
        {
            _logger.LogWarning("Unauthorized access attempt: User ID '{UserId}' is not exist.", request.TargetUserId);
            return new CreateAccount.Response.Unauthorized(request.TargetUserId.ToString());
        }

        int totalUsersAccount = await _context.AccountsRepository.GetTotalByUserId(targetUser.Id, cancellationToken);
        if (totalUsersAccount >= _accountOptions.CurrentValue.MaxAmount)
        {
            _logger.LogWarning(
                "User {UserId} has reached the maximum allowed account limit ({CurrentCount}/{MaxCount}).",
                targetUser.Id.Value,
                totalUsersAccount,
                _accountOptions.CurrentValue.MaxAmount);
            return new CreateAccount.Response.ReachedAccountLimit("You have account amount limit");
        }

        Account? dbAccount = await _context.AccountsRepository
            .FindAccountByNumberAsync(accountNumber, cancellationToken);
        if (dbAccount is not null)
        {
            _logger.LogWarning("Account already exists: Id {accountId}.", dbAccount.Id.Value);
            return new CreateAccount.Response.AccountAlreadyExists(dbAccount.Number.Value);
        }

        var account = new Account(
            new AccountId(_guidProvider.NewGuid()),
            targetUser.Id,
            accountNumber,
            accountPassword,
            Money.Zero,
            _dateTimeProvider.Current,
            _dateTimeProvider.Current);

        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        await _context.AccountsRepository.AddAsync([account], cancellationToken);

        HistoryOperation operation = new CreateAccountHistoryOperation(
            HistoryOperationId.Default,
            account.Id,
            _dateTimeProvider.Current);

        await _context.HistoryOperationsRepository
            .AddAsync([operation], cancellationToken)
            .FirstAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Account {AccountId} created successfully for user {UserId}",
            account.Id.Value,
            targetUser.Id.Value);

        _metrics.IncAccountCreated();

        return new CreateAccount.Response.Success(account.MapToDto());
    }

    public async Task<Deposit.Response> DepositAsync(
        Deposit.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = AccountServiceActivity.ActivitySource.StartActivity();
        activity?.SetTag("account.id", request.AccountId);
        activity?.SetTag("user.id", request.UserId);

        User? user = await _context.UsersRepository.FindByAuthorizationIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Unauthorized access attempt: User ID '{UserId}' is not exist.", request.UserId);
            return new Deposit.Response.Unauthorized(request.UserId);
        }

        var accountId = new AccountId(request.AccountId);
        Account? account = await _context.AccountsRepository.FindAccountByIdAsync(accountId, cancellationToken);
        if (account is null)
        {
            _logger.LogWarning("Account with id {AccountId} not found.", accountId.Value);
            return new Deposit.Response.AccountNotFound(accountId.Value);
        }

        if (account.UserId != user.Id)
        {
            _logger.LogWarning(
                "Account {AccountId} does to belong to the user {UserId}",
                account.Id.Value,
                user.Id.Value);
            return new Deposit.Response.Forbidden("Account is not this users");
        }

        var depositTotal = new Money(request.Amount);
        account.Deposit(depositTotal);
        account.UpdateTime(_dateTimeProvider.Current);
        HistoryOperation operation = new DepositHistoryOperation(
            HistoryOperationId.Default,
            account.Id,
            depositTotal,
            _dateTimeProvider.Current);

        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        await _context.AccountsRepository.UpdateAsync([account], cancellationToken);

        await _context.HistoryOperationsRepository
            .AddAsync([operation], cancellationToken)
            .FirstAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation("Account {AccountId} successfully deposited", account.Id.Value);

        _metrics.IncAccountDeposit();

        return new Deposit.Response.Success(account.MapToDto());
    }

    public async Task<Withdraw.Response> WithdrawAsync(
        Withdraw.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = AccountServiceActivity.ActivitySource.StartActivity();
        activity?.SetTag("account.id", request.AccountId);
        activity?.SetTag("user.id", request.UserId);

        User? user = await _context.UsersRepository.FindByAuthorizationIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Unauthorized access attempt: User ID '{UserId}' is not exist.", request.UserId);
            return new Withdraw.Response.Unauthorized(request.UserId);
        }

        var accountId = new AccountId(request.AccountId);
        Account? account = await _context.AccountsRepository.FindAccountByIdAsync(accountId, cancellationToken);
        if (account is null)
        {
            _logger.LogWarning("Account with id {AccountId} not found.", accountId.Value);
            return new Withdraw.Response.AccountNotFound(accountId.Value);
        }

        if (account.UserId != user.Id)
        {
            _logger.LogWarning(
                "Account {AccountId} does to belong to the user {UserId}",
                account.Id.Value,
                user.Id.Value);
            return new Withdraw.Response.Forbidden("Account is not this users");
        }

        var withdrawTotal = new Money(request.Amount);
        WithdrawResult result = account.Withdraw(withdrawTotal);
        if (result is WithdrawResult.Failure failure)
        {
            _logger.LogWarning("Account {AccountId} withdrawal failure", account.Id.Value);
            return new Withdraw.Response.InsufficientFunds(failure.ErrorMessage);
        }

        account.UpdateTime(_dateTimeProvider.Current);
        HistoryOperation operation = new WithdrawHistoryOperation(
            HistoryOperationId.Default,
            account.Id,
            withdrawTotal,
            _dateTimeProvider.Current);

        await using IPersistenceTransaction transaction = await _transactionProvider
            .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        await _context.AccountsRepository.UpdateAsync([account], cancellationToken);

        await _context.HistoryOperationsRepository
            .AddAsync([operation], cancellationToken)
            .FirstAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation("Account {AccountId} withdrawal successfully", account.Id.Value);

        _metrics.IncAccountWithdrawal();

        return new Withdraw.Response.Success(account.MapToDto());
    }

    public async Task<GetBalance.Response> GetBalanceAsync(
        GetBalance.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = AccountServiceActivity.ActivitySource.StartActivity();
        activity?.SetTag("account.id", request.AccountId);
        activity?.SetTag("user.id", request.UserId);

        User? user = await _context.UsersRepository.FindByAuthorizationIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Unauthorized access attempt: User ID '{UserId}' is not exist.", request.UserId);
            return new GetBalance.Response.Unauthorized(request.UserId);
        }

        var accountId = new AccountId(request.AccountId);
        Account? account = await _context.AccountsRepository.FindAccountByIdAsync(accountId, cancellationToken);
        if (account is null)
        {
            _logger.LogWarning("Account with id {AccountId} not found.", accountId.Value);
            return new GetBalance.Response.AccountNotFound(accountId.Value);
        }

        if (account.UserId != user.Id)
        {
            _logger.LogWarning(
                "Account {AccountId} does to belong to the user {UserId}",
                account.Id.Value,
                user.Id.Value);
            return new GetBalance.Response.Forbidden("Account is not this users");
        }

        var balance = new Money(account.Balance.Value);

        HistoryOperation operation = new CheckBalanceHistoryOperation(
            HistoryOperationId.Default,
            account.Id,
            account.Balance,
            _dateTimeProvider.Current);

        await _context.HistoryOperationsRepository
            .AddAsync([operation], cancellationToken)
            .FirstAsync(cancellationToken);

        return new GetBalance.Response.Success(balance.Value);
    }

    public async Task<GetAccounts.Response> GetAsync(GetAccounts.Request request, CancellationToken cancellationToken)
    {
        using Activity? activity = AccountServiceActivity.ActivitySource.StartActivity();
        activity?.SetTag("user.id", request.UserId);

        User? user = await _context.UsersRepository.FindByAuthorizationIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Unauthorized access attempt: User ID '{UserId}' is not exist.", request.UserId);
            return new GetAccounts.Response.Unauthorized(request.UserId);
        }

        AccountId? idCursor = request.PageToken?.Id is not null
            ? new AccountId(request.PageToken.Value.Id)
            : null;

        var query = AccountQuery.Build(builder => builder
            .WithUserId(user.Id)
            .WithPageSize(request.PageSize)
            .WithAccountIdCursor(idCursor));

        Account[] accounts = await _context.AccountsRepository
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        GetAccounts.PageToken? responsePageToken = accounts.Length < request.PageSize
            ? null
            : new GetAccounts.PageToken(accounts.Last().Id.Value);

        return new GetAccounts.Response.Success(accounts.Select(a => a.MapToDto()).ToArray(), responsePageToken);
    }
}