using Bsa.Domain.Accounts;

namespace Bsa.Domain.Sessions;

public sealed record UserSession
{
    public UserSession(Guid id, AccountId accountId, DateTimeOffset createdTime)
    {
        Id = id;
        AccountId = accountId;
        CreatedTime = createdTime;
    }

    public Guid Id { get; }

    public AccountId AccountId { get; }

    public DateTimeOffset CreatedTime { get; }
}