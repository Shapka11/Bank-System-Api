namespace Bsa.Gateway.Application.Abstractions.Users.Models;

public sealed record BankAdminSessionModel(
    Guid Id,
    DateTimeOffset CreatedAt) : BankSessionBaseModel(Id, CreatedAt);