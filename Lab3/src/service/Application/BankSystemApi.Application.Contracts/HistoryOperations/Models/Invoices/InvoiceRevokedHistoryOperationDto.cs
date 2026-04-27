namespace BankSystemApi.Application.Contracts.HistoryOperations.Models.Invoices;

public sealed record InvoiceRevokedHistoryOperationDto(
    long Id,
    Guid AccountId,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, OccurredAt);