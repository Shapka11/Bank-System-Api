namespace Bsa.Application.Contracts.HistoryOperations.Models.Accounts;

public sealed record CreateAccountHistoryOperationDto(
    long Id,
    long AccountId,
    string AccountNumber,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, AccountNumber, OccurredAt);