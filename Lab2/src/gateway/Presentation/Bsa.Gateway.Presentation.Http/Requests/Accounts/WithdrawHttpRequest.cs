namespace Bsa.Gateway.Presentation.Http.Requests.Accounts;

public readonly record struct WithdrawHttpRequest
{
    public required Guid SessionId { get; init; }

    public required decimal Amount { get; init; }
}