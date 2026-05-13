namespace BankSystemApi.Infrastructure.Persistence.Models.Payloads;

public sealed record InvoicePaymentSentPayload(decimal Amount, long InvoiceId) : PayloadBase;