namespace Bsa.Gateway.Application.Abstractions.Users.Models;

public sealed record BankUserSessionModel(
    Guid Id,
    long AccountId,
    DateTimeOffset CreatedAt) : BankSessionBaseModel(Id, CreatedAt);