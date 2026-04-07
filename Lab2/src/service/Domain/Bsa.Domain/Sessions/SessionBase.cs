namespace Bsa.Domain.Sessions;

public abstract record SessionBase(Guid Id, DateTimeOffset CreatedAt);