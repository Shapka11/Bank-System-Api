namespace BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Models.Invoices;

public sealed record InvoicePaymentSentHistoryOperationDto(
    long Id,
    long AccountId,
    decimal Amount,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, OccurredAt);