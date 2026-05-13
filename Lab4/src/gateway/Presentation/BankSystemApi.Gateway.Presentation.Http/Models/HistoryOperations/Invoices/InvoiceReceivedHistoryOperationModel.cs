namespace BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Invoices;

public sealed record InvoiceReceivedHistoryOperationModel(
    long Id,
    Guid AccountId,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, OccurredAt);