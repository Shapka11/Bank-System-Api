using Bsa.CsharpBackend.Grpc;
using Bsa.Gateway.Application.Abstractions.Users;
using Bsa.Gateway.Application.Abstractions.Users.Admin;
using Bsa.Gateway.Infrastructure.BankService.Mapping;

namespace Bsa.Gateway.Infrastructure.BankService.Clients;

public sealed class AdminClient : IAdminClient
{
    private readonly AdminService.AdminServiceClient _adminClient;

    public AdminClient(AdminService.AdminServiceClient adminClient)
    {
        _adminClient = adminClient;
    }

    public async Task<LoginAdmin.Response> LoginAsync(
        LoginAdmin.Request request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoLoginAdminRequest(request.Password);

        ProtoLoginAdminResponse clientResponse = await _adminClient.LoginAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new LoginAdmin.Response.Success(clientResponse.Session.MapToModel());
    }

    public async Task<LogoutAdmin.Response> LogoutAsync(
        LogoutAdmin.Request request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoLogoutAdminRequest(request.Id.ToString());

        await _adminClient.LogoutAsync(clientRequest, cancellationToken: cancellationToken);

        return new LogoutAdmin.Response.Success();
    }
}