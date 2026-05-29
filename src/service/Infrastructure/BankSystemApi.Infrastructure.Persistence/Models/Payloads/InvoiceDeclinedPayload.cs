namespace BankSystemApi.Infrastructure.Persistence.Models.Payloads;

public sealed record InvoiceDeclinedPayload(long InvoiceId) : PayloadBase;