namespace BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Accounts;

public sealed record DepositHistoryOperationModel(
    long Id,
    long AccountId,
    decimal Amount,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, OccurredAt);