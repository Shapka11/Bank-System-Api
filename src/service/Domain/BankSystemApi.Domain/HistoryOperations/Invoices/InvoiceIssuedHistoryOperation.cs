using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Invoices;

namespace BankSystemApi.Domain.HistoryOperations.Invoices;

public sealed record InvoiceIssuedHistoryOperation(
    HistoryOperationId Id,
    AccountId AccountId,
    InvoiceId InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperation(Id, AccountId, OccurredAt);