using Bsa.Cli.Application.Abstractions.Admin;
using Bsa.Cli.Application.Abstractions.Admin.Operations;
using Bsa.Cli.Application.Contracts.Admin;
using Bsa.Cli.Application.Contracts.Admin.Operations;
using Bsa.Cli.Application.Providers;
using System.Diagnostics;

namespace Bsa.Cli.Application.Services;

public sealed class AdminService : IAdminService
{
    private readonly IAdminClient _adminClient;
    private readonly SessionManager _sessionManager;

    public AdminService(IAdminClient adminClient, SessionManager sessionProvider)
    {
        _adminClient = adminClient;
        _sessionManager = sessionProvider;
    }

    public async Task<LoginAdmin.Result> LoginAdminAsync(
        LoginAdmin.Request request,
        CancellationToken cancellationToken)
    {
        if (_sessionManager.CurrentSessionId is not null)
            return new LoginAdmin.Result.Failure("Session already exist");

        var clientRequest = new LoginAdminQuery.Request(request.Password);
        LoginAdminQuery.Result clientResult = await _adminClient.LoginAdminAsync(clientRequest, cancellationToken);

        if (clientResult is LoginAdminQuery.Result.Success success)
        {
            _sessionManager.Login(success.SessionId);
            return new LoginAdmin.Result.Success();
        }

        if (clientResult is LoginAdminQuery.Result.Failure failure)
        {
            return new LoginAdmin.Result.Failure(failure.ErrorMessage);
        }

        throw new UnreachableException();
    }

    public async Task<LogoutAdmin.Result> LogoutAdminAsync(CancellationToken cancellationToken)
    {
        if (_sessionManager.CurrentSessionId is null)
            return new LogoutAdmin.Result.Failure("Session already log out");

        var clientRequest = new LogoutAdminQuery.Request(_sessionManager.CurrentSessionId.Value);
        LogoutAdminQuery.Result clientResult = await _adminClient.LogoutAdminAsync(clientRequest, cancellationToken);

        if (clientResult is LogoutAdminQuery.Result.Success)
        {
            _sessionManager.Logout();
            return new LogoutAdmin.Result.Success();
        }

        if (clientResult is LogoutAdminQuery.Result.Failure failure)
        {
            return new LogoutAdmin.Result.Failure(failure.ErrorMessage);
        }

        throw new UnreachableException();
    }

    public async Task<CreateAccount.Result> CreateAccountAsync(
        CreateAccount.Request request,
        CancellationToken cancellationToken)
    {
        if (_sessionManager.CurrentSessionId is null)
            return new CreateAccount.Result.Failure("Admin not authorized");

        var clientRequest = new CreateAccountQuery.Request(_sessionManager.CurrentSessionId.Value, request.AccountNumber, request.Password);
        CreateAccountQuery.Result clientResult =
            await _adminClient.CreateAccountAsync(clientRequest, cancellationToken);

        return clientResult switch
        {
            CreateAccountQuery.Result.Success => new CreateAccount.Result.Success(),
            CreateAccountQuery.Result.Failure failure => new CreateAccount.Result.Failure(failure.ErrorMessage),
            _ => throw new UnreachableException(),
        };
    }
}