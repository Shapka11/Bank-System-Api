namespace Bsa.Application.Contracts.Users.Models;

public sealed record UserSessionDto(
    Guid Id,
    long AccountId,
    DateTimeOffset CreatedAt) : SessionBaseDto(Id, CreatedAt);