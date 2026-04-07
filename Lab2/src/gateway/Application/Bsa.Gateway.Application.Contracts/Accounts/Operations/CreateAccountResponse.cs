using Bsa.Gateway.Application.Contracts.Accounts.Models;

namespace Bsa.Gateway.Application.Contracts.Accounts.Operations;

public abstract record CreateAccountResponse
{
    private CreateAccountResponse() { }

    public sealed record Success(AccountDto Account) : CreateAccountResponse;

    public sealed record Failure(string ErrorMessage) : CreateAccountResponse;
}