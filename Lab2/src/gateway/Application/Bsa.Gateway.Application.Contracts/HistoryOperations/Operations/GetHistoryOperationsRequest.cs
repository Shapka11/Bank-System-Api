namespace Bsa.Gateway.Application.Contracts.HistoryOperations.Operations;

public readonly record struct GetHistoryOperationsRequest(Guid Id, int PageSize, string? PageToken);