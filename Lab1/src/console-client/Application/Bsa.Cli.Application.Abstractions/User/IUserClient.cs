using Bsa.Cli.Application.Abstractions.User.Operations;

namespace Bsa.Cli.Application.Abstractions.User;

public interface IUserClient
{
    Task<LoginUserQuery.Result> LoginAsync(LoginUserQuery.Request request, CancellationToken cancellationToken);

    Task<LogoutUserQuery.Result> LogoutAsync(LogoutUserQuery.Request request, CancellationToken cancellationToken);

    Task<DepositQuery.Result> DepositAsync(DepositQuery.Request request, CancellationToken cancellationToken);

    Task<WithdrawQuery.Result> WithdrawAsync(WithdrawQuery.Request request, CancellationToken cancellationToken);

    Task<GetBalanceQuery.Result> GetBalanceAsync(GetBalanceQuery.Request request, CancellationToken cancellationToken);

    Task<GetHistoryQuery.Result> GetHistoryAsync(GetHistoryQuery.Request request, CancellationToken cancellationToken);
}