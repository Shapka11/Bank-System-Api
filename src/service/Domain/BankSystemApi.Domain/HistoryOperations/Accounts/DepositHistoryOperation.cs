using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.ValueObjects;

namespace BankSystemApi.Domain.HistoryOperations.Accounts;

public sealed record DepositHistoryOperation(
    HistoryOperationId Id,
    AccountId AccountId,
    Money Amount,
    DateTimeOffset OccurredAt)
    : HistoryOperation(Id, AccountId, OccurredAt);