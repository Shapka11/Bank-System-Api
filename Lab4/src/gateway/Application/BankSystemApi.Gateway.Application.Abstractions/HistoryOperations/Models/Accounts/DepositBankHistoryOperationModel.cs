namespace BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models.Accounts;

public sealed record DepositBankHistoryOperationModel(
    long Id,
    Guid AccountId,
    decimal Amount,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, OccurredAt);