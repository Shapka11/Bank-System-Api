using Bsa.Domain.Accounts;
using Bsa.Domain.ValueObjects;

namespace Bsa.Domain.HistoryOperations.Accounts;

public sealed record CheckBalanceHistoryOperation(
    HistoryOperationId Id,
    AccountId AccountId,
    AccountNumber AccountNumber,
    Money Balance,
    DateTimeOffset OccurredAt)
    : HistoryOperation(Id, AccountId, AccountNumber, OccurredAt);