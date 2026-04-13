namespace Bsa.Gateway.Presentation.Http.Requests.Accounts;

public sealed class GetBalanceHttpRequest
{
    public required Guid SessionId { get; init; }
}