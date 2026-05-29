using BankSystemApi.Gateway.Application.Abstractions.Accounts.Operations;

namespace BankSystemApi.Gateway.Application.Abstractions.Accounts;

public interface IAccountClient
{
    Task<CreateAccount.Response> CreateAsync(
        CreateAccount.Request request,
        CancellationToken cancellationToken);

    Task<Deposit.Response> DepositAsync(Deposit.Request request, CancellationToken cancellationToken);

    Task<Withdraw.Response> WithdrawAsync(Withdraw.Request request, CancellationToken cancellationToken);

    Task<GetBalance.Response> GetBalanceAsync(GetBalance.Request request, CancellationToken cancellationToken);

    Task<GetAccounts.Response> GetAsync(GetAccounts.Request request, CancellationToken cancellationToken);
}