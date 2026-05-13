namespace BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Accounts;

public sealed record CheckBalanceHistoryOperationModel(
    long Id,
    Guid AccountId,
    decimal Balance,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, OccurredAt);