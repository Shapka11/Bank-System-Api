using BankSystemApi.Application.Contracts.Users;
using BankSystemApi.Application.Contracts.Users.Operations;
using BankSystemApi.Presentation.Grpc.Mapping.Users;
using BankSystemApi.Presentation.Grpc.Mapping.Users.Requests;
using BankSystemApi.Users.Grpc;
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

    public override async Task<ProtoGetUserByIdResponse> IsExists(
        ProtoGetUserByIdRequest request,
        ServerCallContext context)
    {
        GetUsers.Response applicationResponse = await _userService.GetAsyns(
            new GetUsers.Request([], [request.UserId], 1),
            context.CancellationToken);

        return applicationResponse switch
        {
            GetUsers.Response.Success success => new ProtoGetUserByIdResponse(success.Users.Count > 0),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }

    public override async Task<ProtoGetUsersResponse> Get(
        ProtoGetUsersRequest request,
        ServerCallContext context)
    {
        GetUsers.Response applicationResponse = await _userService.GetAsyns(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            GetUsers.Response.Success success => new ProtoGetUsersResponse(
                success.Users.Select(user => user.MapToProto())),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }
}