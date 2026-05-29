using BankSystemApi.Domain.Accounts;

namespace BankSystemApi.Domain.HistoryOperations.Accounts;

public sealed record CreateAccountHistoryOperation(
    HistoryOperationId Id,
    AccountId AccountId,
    DateTimeOffset OccurredAt)
    : HistoryOperation(Id, AccountId, OccurredAt);