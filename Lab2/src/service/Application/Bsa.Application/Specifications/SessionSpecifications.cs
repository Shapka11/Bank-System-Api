using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Domain.Sessions;

namespace Bsa.Application.Specifications;

public sealed class SessionSpecifications
{
    private readonly ISessionRepository _sessionRepository;

    public SessionSpecifications(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<SessionBase?> FindSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = SessionQuery.Build(builder => builder
            .WithSessionId(sessionId)
            .WithPageSize(pageSize));

        return await _sessionRepository.QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }
}