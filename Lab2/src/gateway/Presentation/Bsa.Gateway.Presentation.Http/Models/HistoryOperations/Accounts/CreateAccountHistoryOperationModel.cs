namespace Bsa.Gateway.Presentation.Http.Models.HistoryOperations.Accounts;

public sealed record CreateAccountHistoryOperationModel(
    long Id,
    long AccountId,
    string AccountNumber,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, AccountNumber, OccurredAt);