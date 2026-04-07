namespace Bsa.Gateway.Application.Abstractions.HistoryOperations.Models.Accounts;

public sealed record CheckBalanceBankHistoryOperationModel(
    long Id,
    long AccountId,
    string AccountNumber,
    decimal Balance,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, AccountNumber, OccurredAt);