namespace BankSystemApi.Application.Contracts.HistoryOperations.Models.Invoices;

public sealed record InvoiceIssuedHistoryOperationDto(
    long Id,
    long AccountId,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, OccurredAt);