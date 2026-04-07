namespace Bsa.Gateway.Application.Contracts.HistoryOperations.Models.Invoices;

public sealed record InvoicePaymentSentHistoryOperationDto(
    long Id,
    long AccountId,
    string AccountNumber,
    decimal Amount,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, AccountNumber, OccurredAt);