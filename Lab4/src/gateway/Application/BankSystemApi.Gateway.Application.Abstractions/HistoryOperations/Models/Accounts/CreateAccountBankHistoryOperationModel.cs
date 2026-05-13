namespace BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models.Accounts;

public sealed record CreateAccountBankHistoryOperationModel(
    long Id,
    Guid AccountId,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, OccurredAt);