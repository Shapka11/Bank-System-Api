namespace Bsa.Application.Contracts.HistoryOperations.Models.Invoices;

public sealed record InvoicePaymentReceivedHistoryOperationDto(
    long Id,
    long AccountId,
    string AccountNumber,
    decimal Amount,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, AccountNumber, OccurredAt);