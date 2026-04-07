namespace Bsa.Gateway.Presentation.Http.Models.HistoryOperations.Invoices;

public sealed record InvoiceIssuedHistoryOperationModel(
    long Id,
    long AccountId,
    string AccountNumber,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, AccountNumber, OccurredAt);