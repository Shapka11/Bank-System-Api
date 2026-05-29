namespace BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Accounts;

public sealed record CreateAccountHistoryOperationModel(
    long Id,
    long AccountId,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, OccurredAt);