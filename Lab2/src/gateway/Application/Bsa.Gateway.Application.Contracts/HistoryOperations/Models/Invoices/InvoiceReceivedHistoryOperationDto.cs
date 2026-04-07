namespace Bsa.Gateway.Application.Contracts.HistoryOperations.Models.Invoices;

public sealed record InvoiceReceivedHistoryOperationDto(
    long Id,
    long AccountId,
    string AccountNumber,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, AccountNumber, OccurredAt);