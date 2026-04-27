namespace BankSystemApi.Infrastructure.Persistence.Models.Payloads;

public sealed record InvoiceIssuedPayload(long InvoiceId) : PayloadBase;