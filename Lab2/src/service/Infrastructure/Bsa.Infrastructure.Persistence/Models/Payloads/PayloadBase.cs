using System.Text.Json.Serialization;

namespace Bsa.Infrastructure.Persistence.Models.Payloads;

[JsonDerivedType(typeof(CheckBalancePayload), typeDiscriminator: "check_balance")]
[JsonDerivedType(typeof(CreateAccountPayload), typeDiscriminator: "create_account")]
[JsonDerivedType(typeof(DepositPayload), typeDiscriminator: "deposit")]
[JsonDerivedType(typeof(WithdrawPayload), typeDiscriminator: "withdraw")]
[JsonDerivedType(typeof(InvoiceIssuedPayload), typeDiscriminator: "invoice_issued")]
[JsonDerivedType(typeof(InvoicePaymentReceivedPayload), typeDiscriminator: "invoice_payment_received")]
[JsonDerivedType(typeof(InvoicePaymentSentPayload), typeDiscriminator: "invoice_payment_sent")]
[JsonDerivedType(typeof(InvoiceReceivedPayload), typeDiscriminator: "invoice_recived")]
[JsonDerivedType(typeof(InvoiceRevokedPayload), typeDiscriminator: "invoice_revoked")]
public abstract record PayloadBase();