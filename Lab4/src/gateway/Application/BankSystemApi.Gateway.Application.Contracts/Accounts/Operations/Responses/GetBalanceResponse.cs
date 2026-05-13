namespace BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Responses;

public abstract record GetBalanceResponse
{
    private GetBalanceResponse() { }

    public sealed record Success(decimal Money) : GetBalanceResponse;

    public sealed record Failure(string ErrorMessage) : GetBalanceResponse;
}