namespace Bsa.Infrastructure.Persistence.Models.Payloads;

public sealed record WithdrawPayload(decimal Amount) : PayloadBase;