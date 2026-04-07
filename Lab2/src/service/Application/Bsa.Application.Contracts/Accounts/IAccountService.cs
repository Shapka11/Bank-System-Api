using Bsa.Application.Contracts.Accounts.Operations;

namespace Bsa.Application.Contracts.Accounts;

public interface IAccountService
{
    Task<CreateAccount.Response> CreateAsync(CreateAccount.Request request, CancellationToken cancellationToken);

    Task<Deposit.Response> DepositAsync(
        Deposit.Request request,
        CancellationToken cancellationToken);

    Task<Withdraw.Response> WithdrawAsync(
        Withdraw.Request request,
        CancellationToken cancellationToken);

    Task<GetBalance.Response> GetBalanceAsync(
        GetBalance.Request request,
        CancellationToken cancellationToken);
}