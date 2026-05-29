namespace BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Models;

public abstract record HistoryOperationDto(
    long Id,
    long AccountId,
    DateTimeOffset OccurredAt);