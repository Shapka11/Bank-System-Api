namespace BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Models.Invoices;

public sealed record InvoiceDeclinedHistoryOperationDto(
    long Id,
    long AccountId,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, OccurredAt);