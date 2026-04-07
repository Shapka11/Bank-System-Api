using System.ComponentModel.DataAnnotations;

namespace Bsa.Gateway.Presentation.Http.Requests.HistoryOperations;

public readonly record struct GetHistoryHttpRequest
{
    public required Guid SessionId { get; init; }

    public string? PageToken { get; init; }

    [Range(minimum: 1, maximum: 1000)]
    public int PageSize { get; init; }
}