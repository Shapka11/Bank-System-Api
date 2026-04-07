namespace Bsa.Infrastructure.Persistence.Models.Payloads;

public sealed record DepositPayload(decimal Amount) : PayloadBase;