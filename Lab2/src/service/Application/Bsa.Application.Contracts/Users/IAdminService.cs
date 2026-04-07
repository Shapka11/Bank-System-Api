using Bsa.Application.Contracts.Users.Admin;

namespace Bsa.Application.Contracts.Users;

public interface IAdminService
{
    Task<LoginAdmin.Response> LoginAsync(LoginAdmin.Request request, CancellationToken cancellationToken);

    Task<LogoutAdmin.Response> LogoutAsync(LogoutAdmin.Request request, CancellationToken cancellationToken);
}