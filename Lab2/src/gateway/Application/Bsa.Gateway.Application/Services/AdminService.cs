using Bsa.Gateway.Application.Abstractions.Users;
using Bsa.Gateway.Application.Abstractions.Users.Admin;
using Bsa.Gateway.Application.Contracts.Users;
using Bsa.Gateway.Application.Contracts.Users.Operations;
using Bsa.Gateway.Application.Mapping;
using System.Diagnostics;

namespace Bsa.Gateway.Application.Services;

public sealed class AdminService : IAdminService
{
    private readonly IAdminClient _adminClient;

    public AdminService(IAdminClient adminClient)
    {
        _adminClient = adminClient;
    }

    public async Task<LoginAdminResponse> LoginAdminAsync(
        LoginAdminRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new LoginAdmin.Request(request.Password);
        LoginAdmin.Response clientResponse =
            await _adminClient.LoginAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            LoginAdmin.Response.Failure failure => new LoginAdminResponse.Failure(failure.ErrorMessage),
            LoginAdmin.Response.Success success => new LoginAdminResponse.Success(success.BankAdminSession.MapToDto()),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<LogoutAdminResponse> LogoutAdminAsync(
        LogoutAdminRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new LogoutAdmin.Request(request.Id);
        LogoutAdmin.Response clientResponse =
            await _adminClient.LogoutAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            LogoutAdmin.Response.Failure failure => new LogoutAdminResponse.Failure(failure.ErrorMessage),
            LogoutAdmin.Response.Success => new LogoutAdminResponse.Success(),
            _ => throw new UnreachableException(),
        };
    }
}