namespace Bsa.Gateway.Application.Abstractions.HistoryOperations.Models.Invoices;

public sealed record InvoicePaymentSentBankHistoryOperationModel(
    long Id,
    long AccountId,
    string AccountNumber,
    decimal Amount,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, AccountNumber, OccurredAt);