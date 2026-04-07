namespace Bsa.Application.Contracts.HistoryOperations.Models.Invoices;

public sealed record InvoiceIssuedHistoryOperationDto(
    long Id,
    long AccountId,
    string AccountNumber,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, AccountNumber, OccurredAt);