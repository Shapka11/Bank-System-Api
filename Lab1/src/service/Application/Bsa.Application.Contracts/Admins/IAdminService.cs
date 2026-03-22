using Bsa.Application.Contracts.Admins.Operations;

namespace Bsa.Application.Contracts.Admins;

public interface IAdminService
{
    Task<LoginAdmin.Response> LoginAsync(LoginAdmin.Request request, CancellationToken cancellationToken);

    Task<LogoutAdmin.Response> LogoutAsync(LogoutAdmin.Request request, CancellationToken cancellationToken);

    Task<CreateAccount.Response> CreateAccountAsync(CreateAccount.Request request, CancellationToken cancellationToken);
}