using BankSystemApi.Gateway.Application.Contracts.Accounts.Models;

namespace BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Responses;

public abstract record CreateAccountResponse
{
    private CreateAccountResponse() { }

    public sealed record Success(AccountDto Account) : CreateAccountResponse;

    public sealed record Failure(string ErrorMessage) : CreateAccountResponse;
}