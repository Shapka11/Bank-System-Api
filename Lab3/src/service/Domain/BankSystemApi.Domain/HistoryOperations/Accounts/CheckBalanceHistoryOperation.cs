using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.ValueObjects;

namespace BankSystemApi.Domain.HistoryOperations.Accounts;

public sealed record CheckBalanceHistoryOperation(
    HistoryOperationId Id,
    AccountId AccountId,
    Money Balance,
    DateTimeOffset OccurredAt)
    : HistoryOperation(Id, AccountId, OccurredAt);