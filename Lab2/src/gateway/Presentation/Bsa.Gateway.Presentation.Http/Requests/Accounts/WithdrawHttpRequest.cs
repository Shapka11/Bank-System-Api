namespace Bsa.Gateway.Presentation.Http.Requests.Accounts;

public sealed class WithdrawHttpRequest
{
    public required Guid SessionId { get; init; }

    public required decimal Amount { get; init; }
}