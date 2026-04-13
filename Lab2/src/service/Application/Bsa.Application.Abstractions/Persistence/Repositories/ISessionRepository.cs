using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Domain.Sessions;

namespace Bsa.Application.Abstractions.Persistence.Repositories;

public interface ISessionRepository
{
    Task AddAsync(IReadOnlyCollection<SessionBase> sessions, CancellationToken cancellationToken);

    Task RemoveAsync(IReadOnlyCollection<SessionBase> sessions, CancellationToken cancellationToken);

    IAsyncEnumerable<SessionBase> QueryAsync(SessionQuery query, CancellationToken cancellationToken);
}