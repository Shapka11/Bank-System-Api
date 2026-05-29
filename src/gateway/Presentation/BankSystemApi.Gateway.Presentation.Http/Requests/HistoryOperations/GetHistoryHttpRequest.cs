using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.Gateway.Presentation.Http.Requests.HistoryOperations;

public sealed class GetHistoryHttpRequest
{
    public required long AccountId { get; init; }

    public string? PageToken { get; init; }

    [Range(minimum: 1, maximum: 1000)]
    public required int PageSize { get; init; }
}