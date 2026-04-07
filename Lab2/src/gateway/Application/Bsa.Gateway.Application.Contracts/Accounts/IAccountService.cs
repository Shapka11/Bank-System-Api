using Bsa.Gateway.Application.Contracts.Accounts.Operations;

namespace Bsa.Gateway.Application.Contracts.Accounts;

public interface IAccountService
{
    Task<CreateAccountResponse> CreateAsync(
        CreateAccountRequest request,
        CancellationToken cancellationToken);

    Task<DepositResponse> DepositAsync(DepositRequest request, CancellationToken cancellationToken);

    Task<WithdrawResponse> WithdrawAsync(WithdrawRequest request, CancellationToken cancellationToken);

    Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request, CancellationToken cancellationToken);
}