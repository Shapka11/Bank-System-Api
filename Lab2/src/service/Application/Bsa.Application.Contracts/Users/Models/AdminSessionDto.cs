namespace Bsa.Application.Contracts.Users.Models;

public sealed record AdminSessionDto(
    Guid Id,
    DateTimeOffset CreatedAt) : SessionBaseDto(Id, CreatedAt);