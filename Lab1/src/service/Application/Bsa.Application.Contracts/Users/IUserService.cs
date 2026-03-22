using Bsa.Application.Contracts.Users.Operations;

namespace Bsa.Application.Contracts.Users;

public interface IUserService
{
    Task<LoginUser.Response> LoginAsync(LoginUser.Request request, CancellationToken cancellationToken);

    Task<LogoutUser.Response> LogoutAsync(LogoutUser.Request request, CancellationToken cancellationToken);

    Task<DepositUserAccount.Response> DepositAsync(
        DepositUserAccount.Request request,
        CancellationToken cancellationToken);

    Task<WithdrawUserAccount.Response> WithdrawAsync(
        WithdrawUserAccount.Request request,
        CancellationToken cancellationToken);

    Task<GetUserBalance.Response> GetBalanceAsync(
        GetUserBalance.Request request,
        CancellationToken cancellationToken);

    Task<GetUserOperationHistory.Response> GetHistoryAsync(
        GetUserOperationHistory.Request request,
        CancellationToken cancellationToken);
}