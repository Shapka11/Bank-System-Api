namespace BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models.Invoices;

public sealed record InvoiceIssuedBankHistoryOperationModel(
    long Id,
    Guid AccountId,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, OccurredAt);