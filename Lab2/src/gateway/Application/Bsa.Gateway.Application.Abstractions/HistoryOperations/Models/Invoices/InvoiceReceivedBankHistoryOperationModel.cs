namespace Bsa.Gateway.Application.Abstractions.HistoryOperations.Models.Invoices;

public sealed record InvoiceReceivedBankHistoryOperationModel(
    long Id,
    long AccountId,
    string AccountNumber,
    long InvoiceId,
    DateTimeOffset OccurredAt)
    : BankHistoryOperationModel(Id, AccountId, AccountNumber, OccurredAt);