using BankSystemApi.Domain.Accounts;

namespace BankSystemApi.Domain.HistoryOperations;

public abstract record HistoryOperation(
    HistoryOperationId Id,
    AccountId AccountId,
    DateTimeOffset OccurredAt);