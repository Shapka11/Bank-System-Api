using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.Gateway.Presentation.Http.Requests.Accounts;

public sealed class GetAccountsHttpRequest
{
    public string? PageToken { get; init; }

    [Range(minimum: 1, maximum: 1000)]
    public required int PageSize { get; init; }
}