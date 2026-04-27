namespace BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models.Invoices;

public sealed record InvoicePaymentReceivedBankHistoryOperationModel(
    long Id,
    Guid AccountId,
    decimal Amount,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, OccurredAt);