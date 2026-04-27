using BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Models;

namespace BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Operations;

public abstract record GetHistoryOperationsResponse
{
    private GetHistoryOperationsResponse() { }

    public sealed record Success(
        IReadOnlyCollection<HistoryOperationDto> History,
        string? PageToken) : GetHistoryOperationsResponse;

    public sealed record Failure(string ErrorMessage) : GetHistoryOperationsResponse;
}