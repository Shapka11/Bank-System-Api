using Bsa.Domain.Sessions;
using AdminSessionQuery = Bsa.Application.Abstractions.Persistence.Queries.AdminSessionQuery;

namespace Bsa.Application.Abstractions.Persistence.Repositories;

public interface IAdminSessionRepository
{
    Task AddAsync(IReadOnlyCollection<AdminSession> sessions, CancellationToken cancellationToken);

    Task RemoveAsync(IReadOnlyCollection<AdminSession> sessions, CancellationToken cancellationToken);

    IAsyncEnumerable<AdminSession> QueryAsync(AdminSessionQuery query, CancellationToken cancellationToken);

    Task<AdminSession?> FindAdminSessionAsync(AdminSession adminSession, CancellationToken cancellationToken);
}