namespace BankSystemApi.Application.Contracts.HistoryOperations.Models.Invoices;

public sealed record InvoicePaymentReceivedHistoryOperationDto(
    long Id,
    Guid AccountId,
    decimal Amount,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, OccurredAt);