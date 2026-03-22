using Bsa.Cli.Application.Contracts.Admin.Operations;

namespace Bsa.Cli.Application.Contracts.Admin;

public interface IAdminService
{
    Task<LoginAdmin.Result> LoginAdminAsync(LoginAdmin.Request request, CancellationToken cancellationToken);

    Task<LogoutAdmin.Result> LogoutAdminAsync(CancellationToken cancellationToken);

    Task<CreateAccount.Result> CreateAccountAsync(CreateAccount.Request request, CancellationToken cancellationToken);
}