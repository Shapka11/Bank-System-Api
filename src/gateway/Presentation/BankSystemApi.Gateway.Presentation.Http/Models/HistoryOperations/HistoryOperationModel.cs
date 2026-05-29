using BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Accounts;
using BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations.Invoices;
using System.Text.Json.Serialization;

namespace BankSystemApi.Gateway.Presentation.Http.Models.HistoryOperations;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CheckBalanceHistoryOperationModel), "check_balance")]
[JsonDerivedType(typeof(CreateAccountHistoryOperationModel), "create_account")]
[JsonDerivedType(typeof(DepositHistoryOperationModel), "deposit")]
[JsonDerivedType(typeof(WithdrawHistoryOperationModel), "withdraw")]
[JsonDerivedType(typeof(InvoiceIssuedHistoryOperationModel), "invoice_issued")]
[JsonDerivedType(typeof(InvoicePaymentReceivedHistoryOperationModel), "invoice_payment_received")]
[JsonDerivedType(typeof(InvoicePaymentSentHistoryOperationModel), "invoice_payment_sent")]
[JsonDerivedType(typeof(InvoiceReceivedHistoryOperationModel), "invoice_received")]
[JsonDerivedType(typeof(InvoiceRevokedHistoryOperationModel), "invoice_revoked")]
[JsonDerivedType(typeof(InvoiceApprovedHistoryOperationModel), "invoice_approved")]
[JsonDerivedType(typeof(InvoiceDeclinedHistoryOperationModel), "invoice_declined")]
public abstract record HistoryOperationModel(
    long Id,
    long AccountId,
    DateTimeOffset OccurredAt);