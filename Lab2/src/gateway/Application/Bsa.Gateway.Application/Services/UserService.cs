using Bsa.Gateway.Application.Abstractions.Users;
using Bsa.Gateway.Application.Abstractions.Users.User;
using Bsa.Gateway.Application.Contracts.Users;
using Bsa.Gateway.Application.Contracts.Users.Operations;
using Bsa.Gateway.Application.Mapping;
using System.Diagnostics;

namespace Bsa.Gateway.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserClient _userClient;

    public UserService(IUserClient userClient)
    {
        _userClient = userClient;
    }

    public async Task<LoginUserResponse> LoginUserAsync(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var clientRequest = new LoginUser.Request(request.AccountNumber, request.Password);
        LoginUser.Response clientResponse =
            await _userClient.LoginAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            LoginUser.Response.Failure failure => new LoginUserResponse.Failure(failure.ErrorMessage),
            LoginUser.Response.Success success => new LoginUserResponse.Success(success.BankUserSession.MapToDto()),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<LogoutUserResponse> LogoutUserAsync(
        LogoutUserRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new LogoutUser.Request(request.Id);
        LogoutUser.Response clientResponse =
            await _userClient.LogoutAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            LogoutUser.Response.Failure failure => new LogoutUserResponse.Failure(failure.ErrorMessage),
            LogoutUser.Response.Success => new LogoutUserResponse.Success(),
            _ => throw new UnreachableException(),
        };
    }
}