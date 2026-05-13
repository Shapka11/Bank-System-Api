namespace BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Models.Invoices;

public sealed record InvoiceReceivedHistoryOperationDto(
    long Id,
    Guid AccountId,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperationDto(Id, AccountId, OccurredAt);