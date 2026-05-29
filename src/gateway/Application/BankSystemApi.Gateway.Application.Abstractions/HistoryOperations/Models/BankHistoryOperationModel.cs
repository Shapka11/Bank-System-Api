namespace BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models;

public abstract record BankHistoryOperationModel(
    long Id,
    long AccountId,
    DateTimeOffset OccurredAt);