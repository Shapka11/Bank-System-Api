namespace BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Invoices;

public sealed record InvoicePaymentReceivedHistoryOperationModel(
    long Id,
    Guid AccountId,
    decimal Amount,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, OccurredAt);