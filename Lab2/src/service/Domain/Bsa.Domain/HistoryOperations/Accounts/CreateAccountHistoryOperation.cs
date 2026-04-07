using Bsa.Domain.Accounts;
using Bsa.Domain.ValueObjects;

namespace Bsa.Domain.HistoryOperations.Accounts;

public sealed record CreateAccountHistoryOperation(
    HistoryOperationId Id,
    AccountId AccountId,
    AccountNumber AccountNumber,
    DateTimeOffset OccurredAt)
    : HistoryOperation(Id, AccountId, AccountNumber, OccurredAt);