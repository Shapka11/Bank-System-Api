using Bsa.Gateway.Application.Abstractions.Users.User;

namespace Bsa.Gateway.Application.Abstractions.Users;

public interface IUserClient
{
    Task<LoginUser.Response> LoginAsync(LoginUser.Request request, CancellationToken cancellationToken);

    Task<LogoutUser.Response> LogoutAsync(LogoutUser.Request request, CancellationToken cancellationToken);
}