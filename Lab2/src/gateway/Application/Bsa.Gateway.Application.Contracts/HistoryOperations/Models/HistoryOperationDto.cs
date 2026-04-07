namespace Bsa.Gateway.Application.Contracts.HistoryOperations.Models;

public abstract record HistoryOperationDto(
    long Id,
    long AccountId,
    string AccountNumber,
    DateTimeOffset OccurredAt);