namespace Bsa.Gateway.Application.Abstractions.HistoryOperations.Models.Accounts;

public sealed record CreateAccountBankHistoryOperationModel(
    long Id,
    long AccountId,
    string AccountNumber,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, AccountNumber, OccurredAt);