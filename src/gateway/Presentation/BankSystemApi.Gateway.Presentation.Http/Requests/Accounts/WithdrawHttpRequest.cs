namespace BankSystemApi.Gateway.Presentation.Http.Requests.Accounts;

public sealed class WithdrawHttpRequest
{
    public required long AccountId { get; init; }

    public required decimal Amount { get; init; }
}