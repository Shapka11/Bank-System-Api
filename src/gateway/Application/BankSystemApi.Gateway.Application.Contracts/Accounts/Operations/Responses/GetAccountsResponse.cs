using BankSystemApi.Gateway.Application.Contracts.Accounts.Models;

namespace BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Responses;

public abstract record GetAccountsResponse
{
    private GetAccountsResponse() { }

    public sealed record Success(
        IReadOnlyCollection<AccountDto> Accounts,
        string? PageToken) : GetAccountsResponse;

    public sealed record Failure(string ErrorMessage) : GetAccountsResponse;
}