namespace Bsa.Gateway.Presentation.Http.Models.HistoryOperations.Invoices;

public sealed record InvoiceReceivedHistoryOperationModel(
    long Id,
    long AccountId,
    string AccountNumber,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, AccountNumber, OccurredAt);