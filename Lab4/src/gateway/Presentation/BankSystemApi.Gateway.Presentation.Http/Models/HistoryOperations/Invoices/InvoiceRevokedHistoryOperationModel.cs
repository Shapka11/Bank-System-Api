namespace BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Invoices;

public sealed record InvoiceRevokedHistoryOperationModel(
    long Id,
    Guid AccountId,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationModel(Id, AccountId, OccurredAt);