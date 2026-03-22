using Bsa.Cli.Application.Abstractions.User;
using Bsa.Cli.Application.Abstractions.User.Operations;
using Bsa.Cli.Application.Contracts.User;
using Bsa.Cli.Application.Contracts.User.Operations;
using Bsa.Cli.Application.Mapping;
using Bsa.Cli.Application.Providers;
using System.Diagnostics;

namespace Bsa.Cli.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserClient _userClient;
    private readonly SessionManager _sessionManager;

    public UserService(IUserClient userClient, SessionManager sessionManager)
    {
        _userClient = userClient;
        _sessionManager = sessionManager;
    }

    public async Task<LoginUser.Result> LoginAsync(LoginUser.Request request, CancellationToken cancellationToken)
    {
        if (_sessionManager.CurrentSessionId is not null)
            return new LoginUser.Result.Failure("Session already exist");

        var clientRequest = new LoginUserQuery.Request(request.AccountNumber, request.Password);
        LoginUserQuery.Result clientResult = await _userClient.LoginAsync(clientRequest, cancellationToken);

        if (clientResult is LoginUserQuery.Result.Success success)
        {
            _sessionManager.Login(success.SessionId);
            return new LoginUser.Result.Success();
        }

        if (clientResult is LoginUserQuery.Result.Failure failure)
        {
            return new LoginUser.Result.Failure(failure.ErrorMessage);
        }

        throw new UnreachableException();
    }

    public async Task<LogoutUser.Result> LogoutAsync(CancellationToken cancellationToken)
    {
        if (_sessionManager.CurrentSessionId is null)
            return new LogoutUser.Result.Failure("Session already logged out");

        var clientRequest = new LogoutUserQuery.Request(_sessionManager.CurrentSessionId.Value);
        LogoutUserQuery.Result clientResult = await _userClient.LogoutAsync(clientRequest, cancellationToken);

        if (clientResult is LogoutUserQuery.Result.Success)
        {
            _sessionManager.Logout();
            return new LogoutUser.Result.Success();
        }

        if (clientResult is LogoutUserQuery.Result.Failure failure)
        {
            return new LogoutUser.Result.Failure(failure.ErrorMessage);
        }

        throw new UnreachableException();
    }

    public async Task<Deposit.Result> DepositAsync(Deposit.Request request, CancellationToken cancellationToken)
    {
        if (_sessionManager.CurrentSessionId is null)
            return new Deposit.Result.Failure("Session is not exist");

        var clientRequest = new DepositQuery.Request(_sessionManager.CurrentSessionId.Value, request.Amount);
        DepositQuery.Result clientResult = await _userClient.DepositAsync(clientRequest, cancellationToken);

        return clientResult switch
        {
            DepositQuery.Result.Success => new Deposit.Result.Success(),
            DepositQuery.Result.Failure failure => new Deposit.Result.Failure(failure.ErrorMessage),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<Withdraw.Result> WithdrawAsync(Withdraw.Request request, CancellationToken cancellationToken)
    {
        if (_sessionManager.CurrentSessionId is null)
            return new Withdraw.Result.Failure("Session is not exist");

        var clientRequest = new WithdrawQuery.Request(_sessionManager.CurrentSessionId.Value, request.Amount);
        WithdrawQuery.Result clientResult = await _userClient.WithdrawAsync(clientRequest, cancellationToken);

        return clientResult switch
        {
            WithdrawQuery.Result.Success => new Withdraw.Result.Success(),
            WithdrawQuery.Result.Failure failure => new Withdraw.Result.Failure(failure.ErrorMessage),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetBalance.Result> GetBalanceAsync(CancellationToken cancellationToken)
    {
        if (_sessionManager.CurrentSessionId is null)
            return new GetBalance.Result.Failure("Session is not exist");

        var clientRequest = new GetBalanceQuery.Request(_sessionManager.CurrentSessionId.Value);
        GetBalanceQuery.Result clientResult = await _userClient.GetBalanceAsync(clientRequest, cancellationToken);

        return clientResult switch
        {
            GetBalanceQuery.Result.Success success => new GetBalance.Result.Success(success.Balance),
            GetBalanceQuery.Result.Failure failure => new GetBalance.Result.Failure(failure.ErrorMessage),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetHistory.Result> GetHistoryAsync(CancellationToken cancellationToken)
    {
        if (_sessionManager.CurrentSessionId is null)
            return new GetHistory.Result.Failure("Session is not exist");

        var clientRequest = new GetHistoryQuery.Request(_sessionManager.CurrentSessionId.Value);
        GetHistoryQuery.Result clientResult = await _userClient.GetHistoryAsync(clientRequest, cancellationToken);

        return clientResult switch
        {
            GetHistoryQuery.Result.Success success => new GetHistory.Result.Success(success.History.MapToDto()),
            GetHistoryQuery.Result.Failure failure => new GetHistory.Result.Failure(failure.ErrorMessage),
            _ => throw new UnreachableException(),
        };
    }
}