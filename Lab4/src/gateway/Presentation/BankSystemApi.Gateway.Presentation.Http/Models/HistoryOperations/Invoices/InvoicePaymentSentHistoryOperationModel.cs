namespace BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Invoices;

public sealed record InvoicePaymentSentHistoryOperationModel(
    long Id,
    Guid AccountId,
    decimal Amount,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, OccurredAt);