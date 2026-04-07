using Bsa.Gateway.Application.Contracts.Users.Operations;

namespace Bsa.Gateway.Application.Contracts.Users;

public interface IAdminService
{
    Task<LoginAdminResponse> LoginAdminAsync(LoginAdminRequest request, CancellationToken cancellationToken);

    Task<LogoutAdminResponse> LogoutAdminAsync(
        LogoutAdminRequest request,
        CancellationToken cancellationToken);
}