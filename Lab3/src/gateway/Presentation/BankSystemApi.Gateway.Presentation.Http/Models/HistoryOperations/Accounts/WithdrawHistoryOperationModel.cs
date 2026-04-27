namespace BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Accounts;

public sealed record WithdrawHistoryOperationModel(
    long Id,
    Guid AccountId,
    decimal Amount,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, OccurredAt);