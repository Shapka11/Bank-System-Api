using Bsa.Gateway.Application.Contracts.Accounts.Models;

namespace Bsa.Gateway.Application.Contracts.Accounts.Operations;

public abstract record DepositResponse
{
    private DepositResponse() { }

    public sealed record Success(AccountDto Account) : DepositResponse;

    public sealed record Failure(string ErrorMessage) : DepositResponse;
}