using BankSystemApi.Gateway.Application.Contracts.Accounts.Models;

namespace BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Responses;

public abstract record WithdrawResponse
{
    private WithdrawResponse() { }

    public sealed record Success(AccountDto Account) : WithdrawResponse;

    public sealed record Failure(string ErrorMessage) : WithdrawResponse;
}