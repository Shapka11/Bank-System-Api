using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Domain.ValueObjects;

namespace BankSystemApi.Domain.HistoryOperations.Invoices;

public sealed record InvoicePaymentSentHistoryOperation(
    HistoryOperationId Id,
    AccountId AccountId,
    Money Amount,
    InvoiceId InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperation(Id, AccountId, OccurredAt);