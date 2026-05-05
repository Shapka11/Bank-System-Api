using BankSystemApi.Gateway.Application.Abstractions.Users;
using BankSystemApi.Gateway.Application.Abstractions.Users.Operations;
using BankSystemApi.Gateway.Infrastructure.BankService.Activities;
using BankSystemApi.Gateway.Infrastructure.BankService.Extensions;
using BankSystemApi.Grpc;
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

        await _userClient.AddAsync(clientRequest, cancellationToken: cancellationToken);

        return new AddUser.Response.Success();
    }
}