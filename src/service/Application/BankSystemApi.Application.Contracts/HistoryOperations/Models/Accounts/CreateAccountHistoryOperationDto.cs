namespace BankSystemApi.Application.Contracts.HistoryOperations.Models.Accounts;

public sealed record CreateAccountHistoryOperationDto(
    long Id,
    long AccountId,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, OccurredAt);