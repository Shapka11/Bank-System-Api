namespace BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models;

public abstract record BankHistoryOperationModel(
    long Id,
    Guid AccountId,
    DateTimeOffset OccurredAt);