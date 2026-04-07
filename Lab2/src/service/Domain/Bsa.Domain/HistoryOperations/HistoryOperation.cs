using Bsa.Domain.Accounts;
using Bsa.Domain.ValueObjects;

namespace Bsa.Domain.HistoryOperations;

public abstract record HistoryOperation(
    HistoryOperationId Id,
    AccountId AccountId,
    AccountNumber AccountNumber,
    DateTimeOffset OccurredAt);