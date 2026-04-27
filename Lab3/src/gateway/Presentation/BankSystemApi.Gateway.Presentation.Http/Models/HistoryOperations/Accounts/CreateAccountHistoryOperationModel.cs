namespace BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Accounts;

public sealed record CreateAccountHistoryOperationModel(
    long Id,
    Guid AccountId,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, OccurredAt);