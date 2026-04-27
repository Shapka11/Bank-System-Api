using BankSystemApi.Application.Contracts.Users;
using BankSystemApi.Application.Contracts.Users.Operations;
using BankSystemApi.Grpc;
using BankSystemApi.Presentation.Grpc.Mapping.Users.Requests;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace BankSystemApi.Presentation.Grpc.Controllers;

public sealed class UserController : UserService.UserServiceBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<Empty> Add(
        ProtoAddUserRequest request,
        ServerCallContext context)
    {
        AddUser.Response applicationResponse = await _userService.AddAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            AddUser.Response.Success => new Empty(),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }
}