using BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations;

namespace BankSystemApi.Gateway.Presentation.Http.Responses.HistoryOperations;

public readonly record struct GetHistoryHttpResponse
{
    public required IReadOnlyCollection<HistoryOperationModel> History { get; init; }

    public string? PageToken { get; init; }
}