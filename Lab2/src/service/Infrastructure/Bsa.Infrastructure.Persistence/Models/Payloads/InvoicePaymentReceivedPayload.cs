namespace Bsa.Infrastructure.Persistence.Models.Payloads;

public sealed record InvoicePaymentReceivedPayload(decimal Amount, long InvoiceId) : PayloadBase;