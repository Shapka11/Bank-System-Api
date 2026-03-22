using Bsa.Cli.Application.Abstractions.Admin.Operations;

namespace Bsa.Cli.Application.Abstractions.Admin;

public interface IAdminClient
{
    Task<LoginAdminQuery.Result> LoginAdminAsync(LoginAdminQuery.Request request, CancellationToken cancellationToken);

    Task<LogoutAdminQuery.Result> LogoutAdminAsync(LogoutAdminQuery.Request request, CancellationToken cancellationToken);

    Task<CreateAccountQuery.Result> CreateAccountAsync(CreateAccountQuery.Request request, CancellationToken cancellationToken);
}