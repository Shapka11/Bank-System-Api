using BankSystemApi.Application.Contracts.Users;
using BankSystemApi.Application.Contracts.Users.Operations;
using BankSystemApi.Grpc;
using BankSystemApi.Presentation.Grpc.Mapping.Users;
using BankSystemApi.Presentation.Grpc.Mapping.Users.Requests;
using Grpc.Core;

namespace BankSystemApi.Presentation.Grpc.Controllers;

public sealed class UserController : UserService.UserServiceBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<ProtoAddUserResponse> Add(
        ProtoAddUserRequest request,
        ServerCallContext context)
    {
        AddUser.Response applicationResponse = await _userService.AddAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            AddUser.Response.Success success => new ProtoAddUserResponse(success.User.MapToProto()),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }
}