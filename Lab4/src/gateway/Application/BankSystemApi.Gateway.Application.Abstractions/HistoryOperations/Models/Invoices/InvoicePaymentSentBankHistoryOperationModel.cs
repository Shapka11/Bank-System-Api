namespace BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models.Invoices;

public sealed record InvoicePaymentSentBankHistoryOperationModel(
    long Id,
    Guid AccountId,
    decimal Amount,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, OccurredAt);