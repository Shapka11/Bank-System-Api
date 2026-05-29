namespace BankSystemApi.Infrastructure.Persistence.Models.Payloads;

public sealed record InvoiceRevokedPayload(long InvoiceId) : PayloadBase;