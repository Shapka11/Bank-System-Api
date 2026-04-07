using Bsa.CsharpBackend.Grpc;
using Bsa.Gateway.Application.Abstractions.Users;
using Bsa.Gateway.Application.Abstractions.Users.User;
using Bsa.Gateway.Infrastructure.BankService.Mapping;

namespace Bsa.Gateway.Infrastructure.BankService.Clients;

public sealed class UserClient : IUserClient
{
    private readonly UserService.UserServiceClient _userClient;

    public UserClient(UserService.UserServiceClient userClient)
    {
        _userClient = userClient;
    }

    public async Task<LoginUser.Response> LoginAsync(LoginUser.Request request, CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoLoginUserRequest(request.AccountNumber, request.Password);

        ProtoLoginUserResponse clientResponse = await _userClient.LoginAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new LoginUser.Response.Success(clientResponse.Session.MapToModel());
    }

    public async Task<LogoutUser.Response> LogoutAsync(
        LogoutUser.Request request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoLogoutUserRequest(request.Id.ToString());

        await _userClient.LogoutAsync(clientRequest, cancellationToken: cancellationToken);

        return new LogoutUser.Response.Success();
    }
}