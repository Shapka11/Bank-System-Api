using Bsa.Application.Contracts.Users;
using Bsa.Application.Contracts.Users.User;
using Bsa.CsharpBackend.Grpc;
using Bsa.Presentation.Grpc.Mapping.Users;
using Bsa.Presentation.Grpc.Mapping.Users.User;
using Grpc.Core;

namespace Bsa.Presentation.Grpc.Controllers;

public sealed class UserController : UserService.UserServiceBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<ProtoLoginUserResponse> Login(
        ProtoLoginUserRequest request,
        ServerCallContext context)
    {
        LoginUser.Response applicationResponse = await _userService.LoginAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            LoginUser.Response.Success success => new ProtoLoginUserResponse(success.UserSession.MapToProto()),
            LoginUser.Response.AccountNotFound accountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Account {accountNotFound.AccountNumber} not found")),
            LoginUser.Response.InvalidPassword => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                "Invalid account password")),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }

    public override async Task<ProtoLogoutUserResponse> Logout(
        ProtoLogoutUserRequest request,
        ServerCallContext context)
    {
        LogoutUser.Response applicationResponse = await _userService.LogoutAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            LogoutUser.Response.Success => new ProtoLogoutUserResponse(),
            LogoutUser.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                unauthorized.ErrorMessage)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }
}