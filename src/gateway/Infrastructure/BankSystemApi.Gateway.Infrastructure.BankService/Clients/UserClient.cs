using BankSystemApi.Gateway.Application.Abstractions.Users;
using BankSystemApi.Gateway.Application.Abstractions.Users.Operations;
using BankSystemApi.Gateway.Infrastructure.BankService.Activities;
using BankSystemApi.Gateway.Infrastructure.BankService.Extensions;
using BankSystemApi.Gateway.Infrastructure.BankService.Mapping;
using BankSystemApi.Users.Grpc;
using System.Diagnostics;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Clients;

public sealed class UserClient : IUserClient
{
    private readonly UserService.UserServiceClient _userClient;

    public UserClient(UserService.UserServiceClient userClient)
    {
        _userClient = userClient;
    }

    public async Task<AddUser.Response> AddAsync(AddUser.Request request, CancellationToken cancellationToken)
    {
        using Activity? activity = UserClientActivity.ActivitySource.StartActivity();
        activity.AddUserIdBaggage(request.AuthorizationId);

        var clientRequest = new ProtoAddUserRequest(request.AuthorizationId.ToString());

        ProtoAddUserResponse response = await _userClient.AddAsync(clientRequest, cancellationToken: cancellationToken);

        return new AddUser.Response.Success(response.User.MapToModel());
    }

    public async Task<GetUsers.Response> GetAsync(GetUsers.Request request, CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoGetUsersRequest(
            request.AuthorizationIds.Select(id => id.ToString()),
            request.UserIds,
            request.PageSize);

        ProtoGetUsersResponse response = await _userClient
            .GetAsync(clientRequest, cancellationToken: cancellationToken);

        return new GetUsers.Response.Success(response.Users.Select(user => user.MapToModel()).ToArray());
    }
}