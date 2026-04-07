using Bsa.Application.Contracts.Users.User;

namespace Bsa.Application.Contracts.Users;

public interface IUserService
{
    Task<LoginUser.Response> LoginAsync(LoginUser.Request request, CancellationToken cancellationToken);

    Task<LogoutUser.Response> LogoutAsync(LogoutUser.Request request, CancellationToken cancellationToken);
}