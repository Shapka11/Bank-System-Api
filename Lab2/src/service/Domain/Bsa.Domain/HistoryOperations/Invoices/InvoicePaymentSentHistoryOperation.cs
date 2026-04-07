using Bsa.Domain.Accounts;
using Bsa.Domain.Invoices;
using Bsa.Domain.ValueObjects;

namespace Bsa.Domain.HistoryOperations.Invoices;

public sealed record InvoicePaymentSentHistoryOperation(
    HistoryOperationId Id,
    AccountId AccountId,
    AccountNumber AccountNumber,
    Money Amount,
    InvoiceId InvoiceId,
    DateTimeOffset OccurredAt)
    : HistoryOperation(Id, AccountId, AccountNumber, OccurredAt);