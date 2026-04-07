namespace Bsa.Application.Contracts.HistoryOperations.Models.Accounts;

public sealed record WithdrawHistoryOperationDto(
    long Id,
    long AccountId,
    string AccountNumber,
    decimal Amount,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, AccountNumber, OccurredAt);