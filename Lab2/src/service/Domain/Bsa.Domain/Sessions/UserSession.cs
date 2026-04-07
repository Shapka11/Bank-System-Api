using Bsa.Domain.Accounts;

namespace Bsa.Domain.Sessions;

public sealed record UserSession(
    Guid Id,
    AccountId AccountId,
    DateTimeOffset CreatedAt) : SessionBase(Id, CreatedAt);