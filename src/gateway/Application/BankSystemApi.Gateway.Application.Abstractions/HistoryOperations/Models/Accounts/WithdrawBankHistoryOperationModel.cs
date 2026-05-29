namespace BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models.Accounts;

public sealed record WithdrawBankHistoryOperationModel(
    long Id,
    long AccountId,
    decimal Amount,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, OccurredAt);