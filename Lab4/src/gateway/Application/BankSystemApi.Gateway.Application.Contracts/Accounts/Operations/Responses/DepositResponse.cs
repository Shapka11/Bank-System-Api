using BankSystemApi.Gateway.Application.Contracts.Accounts.Models;

namespace BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Responses;

public abstract record DepositResponse
{
    private DepositResponse() { }

    public sealed record Success(AccountDto Account) : DepositResponse;

    public sealed record Failure(string ErrorMessage) : DepositResponse;
}