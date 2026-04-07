using Bsa.Gateway.Application.Abstractions.Users.Admin;

namespace Bsa.Gateway.Application.Abstractions.Users;

public interface IAdminClient
{
    Task<LoginAdmin.Response> LoginAsync(LoginAdmin.Request request, CancellationToken cancellationToken);

    Task<LogoutAdmin.Response> LogoutAsync(
        LogoutAdmin.Request request,
        CancellationToken cancellationToken);
}