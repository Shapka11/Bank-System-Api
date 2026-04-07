namespace Bsa.Gateway.Application.Abstractions.HistoryOperations.Models.Accounts;

public sealed record WithdrawBankHistoryOperationModel(
    long Id,
    long AccountId,
    string AccountNumber,
    decimal Amount,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, AccountNumber, OccurredAt);