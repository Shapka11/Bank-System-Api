using Bsa.Cli.Infrastructure.ClientService.Admins.Models;
using Refit;

namespace Bsa.Cli.Infrastructure.ClientService.Admins;

public interface IRefitAdminClient
{
    [Post("/api/admin/login")]
    Task<IApiResponse<LoginAdminResponse>> LoginAsync(
        [Body] LoginAdminRequest request,
        CancellationToken cancellationToken);

    [Post("/api/admin/logout")]
    Task<IApiResponse> LogoutAsync(
        [Body] LogoutAdminRequest request,
        CancellationToken cancellationToken);

    [Post("/api/admin/account")]
    Task<IApiResponse> CreateAccountAsync(
        [Body] CreateAccountRequest request,
        CancellationToken cancellationToken);
}