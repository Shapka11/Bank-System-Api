namespace BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models.Accounts;

public sealed record DepositBankHistoryOperationModel(
    long Id,
    long AccountId,
    decimal Amount,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, OccurredAt);