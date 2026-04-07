using Bsa.Gateway.Presentation.Http.Models.Accounts;

namespace Bsa.Gateway.Presentation.Http.Responses.Accounts;

public readonly record struct DepositHttpResponse
{
    public required AccountModel Account { get; init; }
}