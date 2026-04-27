namespace BankSystemApi.Application.Contracts.HistoryOperations.Models.Accounts;

public sealed record WithdrawHistoryOperationDto(
    long Id,
    Guid AccountId,
    decimal Amount,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, OccurredAt);