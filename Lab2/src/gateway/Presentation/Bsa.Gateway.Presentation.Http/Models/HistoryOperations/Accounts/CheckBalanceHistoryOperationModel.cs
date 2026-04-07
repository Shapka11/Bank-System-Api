namespace Bsa.Gateway.Presentation.Http.Models.HistoryOperations.Accounts;

public sealed record CheckBalanceHistoryOperationModel(
    long Id,
    long AccountId,
    string AccountNumber,
    decimal Balance,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, AccountNumber, OccurredAt);