namespace BankSystemApi.Infrastructure.Persistence.Models.Payloads;

public sealed record CheckBalancePayload(decimal Balance) : PayloadBase;