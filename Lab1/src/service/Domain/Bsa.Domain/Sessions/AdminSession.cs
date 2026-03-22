namespace Bsa.Domain.Sessions;

public sealed record AdminSession
{
    public AdminSession(Guid id, DateTimeOffset createdTime)
    {
        Id = id;
        CreatedTime = createdTime;
    }

    public Guid Id { get; }

    public DateTimeOffset CreatedTime { get; }
}