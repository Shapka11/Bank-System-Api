using Bsa.Gateway.Application.Contracts.Accounts.Models;

namespace Bsa.Gateway.Application.Contracts.Accounts.Operations;

public abstract record WithdrawResponse
{
    private WithdrawResponse() { }

    public sealed record Success(AccountDto Account) : WithdrawResponse;

    public sealed record Failure(string ErrorMessage) : WithdrawResponse;
}