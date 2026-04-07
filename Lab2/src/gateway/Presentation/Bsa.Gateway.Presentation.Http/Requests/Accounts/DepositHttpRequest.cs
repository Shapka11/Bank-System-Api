namespace Bsa.Gateway.Presentation.Http.Requests.Accounts;

public readonly record struct DepositHttpRequest
{
    public required Guid SessionId { get; init; }

    public required decimal Amount { get; init; }
}