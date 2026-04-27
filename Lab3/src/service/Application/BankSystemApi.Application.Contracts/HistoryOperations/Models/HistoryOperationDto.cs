namespace BankSystemApi.Application.Contracts.HistoryOperations.Models;

public abstract record HistoryOperationDto(
    long Id,
    Guid AccountId,
    DateTimeOffset OccurredAt);