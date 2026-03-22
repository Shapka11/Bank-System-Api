using Bsa.Cli.Application.Contracts.User.Operations;

namespace Bsa.Cli.Application.Contracts.User;

public interface IUserService
{
    Task<LoginUser.Result> LoginAsync(LoginUser.Request request, CancellationToken cancellationToken);

    Task<LogoutUser.Result> LogoutAsync(CancellationToken cancellationToken);

    Task<Deposit.Result> DepositAsync(Deposit.Request request, CancellationToken cancellationToken);

    Task<Withdraw.Result> WithdrawAsync(Withdraw.Request request, CancellationToken cancellationToken);

    Task<GetBalance.Result> GetBalanceAsync(CancellationToken cancellationToken);

    Task<GetHistory.Result> GetHistoryAsync(CancellationToken cancellationToken);
}