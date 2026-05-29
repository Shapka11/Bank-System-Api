namespace BankSystemApi.Application.Contracts.HistoryOperations.Models.Accounts;

public sealed record CheckBalanceHistoryOperationDto(
    long Id,
    long AccountId,
    decimal Balance,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, OccurredAt);