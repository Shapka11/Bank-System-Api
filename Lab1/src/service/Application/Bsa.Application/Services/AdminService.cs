using Bsa.Application.Abstractions.Persistence;
using Bsa.Application.Contracts.Admins;
using Bsa.Application.Contracts.Admins.Operations;
using Bsa.Application.Mapping;
using Bsa.Application.Options;
using Bsa.Application.Providers;
using Bsa.Domain.Accounts;
using Bsa.Domain.Operations;
using Bsa.Domain.Sessions;
using Bsa.Domain.ValueObjects;
using Microsoft.Extensions.Options;
using System.Transactions;

namespace Bsa.Application.Services;

public sealed class AdminService : IAdminService
{
    private readonly IPersistenceContext _context;
    private readonly IOptionsMonitor<SecurityOptions> _options;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AdminService(
        IPersistenceContext context,
        IOptionsMonitor<SecurityOptions> options,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _options = options;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<LoginAdmin.Response> LoginAsync(LoginAdmin.Request request, CancellationToken cancellationToken)
    {
        var password = new Password(request.Password);
        var systemPassword = new Password(_options.CurrentValue.SystemPassword);

        if (password != systemPassword)
            return new LoginAdmin.Response.Failure("Wrong password");

        var session = new AdminSession(Guid.NewGuid(), _dateTimeProvider.Current);
        await _context.AdminSessionsRepository.AddAsync([session], cancellationToken);

        return new LoginAdmin.Response.Success(session.MapToDto());
    }

    public async Task<LogoutAdmin.Response> LogoutAsync(
        LogoutAdmin.Request request,
        CancellationToken cancellationToken)
    {
        var requestAdminSession = new AdminSession(request.Id, _dateTimeProvider.Current);
        AdminSession? dbAdminSession = await _context.AdminSessionsRepository
            .FindAdminSessionAsync(requestAdminSession, cancellationToken);

        if (dbAdminSession is null)
            return new LogoutAdmin.Response.Failure("Admin session not found");

        await _context.AdminSessionsRepository.RemoveAsync([requestAdminSession], cancellationToken);

        return new LogoutAdmin.Response.Success();
    }

    public async Task<CreateAccount.Response> CreateAccountAsync(
        CreateAccount.Request request,
        CancellationToken cancellationToken)
    {
        var requestAdminSession = new AdminSession(request.Id, _dateTimeProvider.Current);
        AdminSession? dbAdminSession = await _context.AdminSessionsRepository
            .FindAdminSessionAsync(requestAdminSession, cancellationToken);

        if (dbAdminSession is null)
            return new CreateAccount.Response.Failure("Admin session not found");

        Account? dbAccount = await _context.AccountsRepository
            .FindAccountByNumberAsync(new AccountNumber(request.AccountNumber), cancellationToken);

        if (dbAccount is not null)
            return new CreateAccount.Response.Failure("Account already exists");

        var account = new Account(
            AccountId.Default,
            new AccountNumber(request.AccountNumber),
            new Password(request.Password),
            Money.Zero,
            _dateTimeProvider.Current,
            _dateTimeProvider.Current);

        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        account = await _context.AccountsRepository
            .AddAsync([account], cancellationToken)
            .FirstAsync(cancellationToken);

        var operation = new AccountOperation(
            AccountOperationId.Default,
            account.Id,
            account.Number,
            account.Balance,
            AccountOperationType.Create,
            _dateTimeProvider.Current);

        await _context.AccountOperationsRepository
            .AddAsync([operation], cancellationToken)
            .FirstAsync(cancellationToken);

        transaction.Complete();
        return new CreateAccount.Response.Success(account.MapToDto());
    }
}