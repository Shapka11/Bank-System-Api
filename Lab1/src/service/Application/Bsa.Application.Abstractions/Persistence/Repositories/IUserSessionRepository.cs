using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Domain.Sessions;

namespace Bsa.Application.Abstractions.Persistence.Repositories;

public interface IUserSessionRepository
{
    Task AddAsync(IReadOnlyCollection<UserSession> sessions, CancellationToken cancellationToken);

    Task RemoveAsync(IReadOnlyCollection<UserSession> sessions, CancellationToken cancellationToken);

    IAsyncEnumerable<UserSession> QueryAsync(UserSessionQuery query, CancellationToken cancellationToken);

    Task<UserSession?> FindUserSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken);
}