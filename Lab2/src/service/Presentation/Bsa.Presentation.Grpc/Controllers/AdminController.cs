using Bsa.Application.Contracts.Users;
using Bsa.Application.Contracts.Users.Admin;
using Bsa.CsharpBackend.Grpc;
using Bsa.Presentation.Grpc.Mapping.Users;
using Bsa.Presentation.Grpc.Mapping.Users.Admin;
using Grpc.Core;

namespace Bsa.Presentation.Grpc.Controllers;

public sealed class AdminController : AdminService.AdminServiceBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public override async Task<ProtoLoginAdminResponse> Login(
        ProtoLoginAdminRequest request,
        ServerCallContext context)
    {
        LoginAdmin.Response applicationResponse = await _adminService.LoginAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            LoginAdmin.Response.Success success => new ProtoLoginAdminResponse(success.AdminSession.MapToProto()),
            LoginAdmin.Response.InvalidPassword => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                "Invalid system password")),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }

    public override async Task<ProtoLogoutAdminResponse> Logout(
        ProtoLogoutAdminRequest request,
        ServerCallContext context)
    {
        LogoutAdmin.Response applicationResponse = await _adminService.LogoutAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            LogoutAdmin.Response.Success => new ProtoLogoutAdminResponse(),
            LogoutAdmin.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                unauthorized.ErrorMessage)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }
}