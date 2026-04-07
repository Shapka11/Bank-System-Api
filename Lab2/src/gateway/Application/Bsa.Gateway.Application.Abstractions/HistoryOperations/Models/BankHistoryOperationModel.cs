namespace Bsa.Gateway.Application.Abstractions.HistoryOperations.Models;

public abstract record BankHistoryOperationModel(
    long Id,
    long AccountId,
    string AccountNumber,
    DateTimeOffset OccurredAt);