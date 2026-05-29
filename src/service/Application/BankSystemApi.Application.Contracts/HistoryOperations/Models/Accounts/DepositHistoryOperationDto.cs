namespace BankSystemApi.Application.Contracts.HistoryOperations.Models.Accounts;

public sealed record DepositHistoryOperationDto(
    long Id,
    long AccountId,
    decimal Amount,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, OccurredAt);