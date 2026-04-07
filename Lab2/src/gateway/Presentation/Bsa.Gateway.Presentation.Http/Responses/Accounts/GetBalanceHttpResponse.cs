namespace Bsa.Gateway.Presentation.Http.Responses.Accounts;

public readonly record struct GetBalanceHttpResponse
{
    public required decimal Balance { get; init; }
}