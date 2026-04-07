namespace Bsa.Domain.Sessions;

public sealed record AdminSession(
    Guid Id,
    DateTimeOffset CreatedAt) : SessionBase(Id, CreatedAt);