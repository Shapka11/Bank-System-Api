namespace Bsa.Infrastructure.Persistence.Models.Payloads;

public sealed record InvoiceReceivedPayload(long InvoiceId) : PayloadBase;