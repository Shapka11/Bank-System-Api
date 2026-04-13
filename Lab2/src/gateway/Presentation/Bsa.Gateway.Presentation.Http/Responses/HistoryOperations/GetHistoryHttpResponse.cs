using Bsa.Gateway.Presentation.Http.Models.HistoryOperations;

namespace Bsa.Gateway.Presentation.Http.Responses.HistoryOperations;

public readonly record struct GetHistoryHttpResponse
{
    public required IReadOnlyCollection<HistoryOperationModel> History { get; init; }

    public string? PageToken { get; init; }
}