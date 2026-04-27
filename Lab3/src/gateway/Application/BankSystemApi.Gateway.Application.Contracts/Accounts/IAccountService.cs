using BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Requests;
using BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Responses;

namespace BankSystemApi.Gateway.Application.Contracts.Accounts;

public interface IAccountService
{
    Task<CreateAccountResponse> CreateAsync(
        CreateAccountRequest request,
        CancellationToken cancellationToken);

    Task<DepositResponse> DepositAsync(DepositRequest request, CancellationToken cancellationToken);

    Task<WithdrawResponse> WithdrawAsync(WithdrawRequest request, CancellationToken cancellationToken);

    Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request, CancellationToken cancellationToken);

    Task<GetAccountsResponse> GetAccountAsync(GetAccountsRequest request, CancellationToken cancellationToken);
}