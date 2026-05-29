namespace BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Operations;

public readonly record struct GetHistoryOperationsRequest(
    Guid UserId,
    long AccountId,
    int PageSize,
    string? PageToken);