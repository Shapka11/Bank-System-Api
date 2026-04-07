namespace Bsa.Gateway.Presentation.Http.Models.HistoryOperations.Invoices;

public sealed record InvoicePaymentReceivedHistoryOperationModel(
    long Id,
    long AccountId,
    string AccountNumber,
    decimal Amount,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, AccountNumber, OccurredAt);