using Bsa.Gateway.Application.Contracts.Users.Operations;

namespace Bsa.Gateway.Application.Contracts.Users;

public interface IUserService
{
    Task<LoginUserResponse> LoginUserAsync(LoginUserRequest request, CancellationToken cancellationToken);

    Task<LogoutUserResponse> LogoutUserAsync(LogoutUserRequest request, CancellationToken cancellationToken);
}