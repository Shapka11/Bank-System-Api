namespace Bsa.Gateway.Presentation.Http.Models.HistoryOperations.Accounts;

public sealed record DepositHistoryOperationModel(
    long Id,
    long AccountId,
    string AccountNumber,
    decimal Amount,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, AccountNumber, OccurredAt);