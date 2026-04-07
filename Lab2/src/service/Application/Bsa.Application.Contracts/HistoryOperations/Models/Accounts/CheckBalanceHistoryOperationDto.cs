namespace Bsa.Application.Contracts.HistoryOperations.Models.Accounts;

public sealed record CheckBalanceHistoryOperationDto(
    long Id,
    long AccountId,
    string AccountNumber,
    decimal Balance,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, AccountNumber, OccurredAt);