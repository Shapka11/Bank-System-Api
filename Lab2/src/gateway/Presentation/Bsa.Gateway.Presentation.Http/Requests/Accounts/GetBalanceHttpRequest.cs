namespace Bsa.Gateway.Presentation.Http.Requests.Accounts;

public readonly record struct GetBalanceHttpRequest
{
    public required Guid SessionId { get; init; }
}