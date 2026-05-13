namespace BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models.Invoices;

public sealed record InvoiceRevokedBankHistoryOperationModel(
    long Id,
    Guid AccountId,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, OccurredAt);