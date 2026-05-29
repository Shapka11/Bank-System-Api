namespace BankSystemApi.Infrastructure.Persistence.Models.Payloads;

public sealed record InvoiceApprovedPayload(long InvoiceId) : PayloadBase;