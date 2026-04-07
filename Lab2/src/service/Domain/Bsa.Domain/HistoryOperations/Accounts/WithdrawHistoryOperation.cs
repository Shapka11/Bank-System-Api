using Bsa.Domain.Accounts;
using Bsa.Domain.ValueObjects;

namespace Bsa.Domain.HistoryOperations.Accounts;

public sealed record WithdrawHistoryOperation(
    HistoryOperationId Id,
    AccountId AccountId,
    AccountNumber AccountNumber,
    Money Amount,
    DateTimeOffset OccurredAt)
    : HistoryOperation(Id, AccountId, AccountNumber, OccurredAt);