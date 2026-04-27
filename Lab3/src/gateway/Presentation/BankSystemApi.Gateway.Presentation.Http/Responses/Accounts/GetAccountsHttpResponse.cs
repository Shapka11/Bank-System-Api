using BankSystemApi.Gateway.Presentation.Http.Models.Accounts;

namespace BankSystemApi.Gateway.Presentation.Http.Responses.Accounts;

public readonly record struct GetAccountsHttpResponse
{
    public required IReadOnlyCollection<AccountModel> Accounts { get; init; }

    public string? PageToken { get; init; }
}