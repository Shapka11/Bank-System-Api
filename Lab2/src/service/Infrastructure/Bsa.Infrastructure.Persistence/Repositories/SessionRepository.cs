using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Domain.Accounts;
using Bsa.Domain.Sessions;
using Bsa.Infrastructure.Persistence.Specifications;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Bsa.Infrastructure.Persistence.Repositories;

public sealed class SessionRepository : ISessionRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public SessionRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task AddAsync(IReadOnlyCollection<SessionBase> sessions, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO sessions (id, account_id, created_at)
        SELECT id, account_id, created_at
        FROM unnest(:ids, :accountIds, :createdAts) 
            AS source(id, account_id, created_at)
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        var accountIds = sessions.Select(s => s is UserSession userSession
                ? userSession.AccountId.Value
                : (long?)null)
            .ToList();

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", sessions.Select(s => s.Id))
            .AddParameter("accountIds", accountIds)
            .AddParameter("createdAts", sessions.Select(s => s.CreatedAt));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task RemoveAsync(IReadOnlyCollection<SessionBase> sessions, CancellationToken cancellationToken)
    {
        const string sql = """
        DELETE FROM sessions 
        WHERE id = ANY(:ids)
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", sessions.Select(s => s.Id));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<SessionBase> QueryAsync(
        SessionQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, account_id, created_at
        FROM sessions
        WHERE
          (:cursor IS NULL OR id > :cursor)
          AND (cardinality(:ids) = 0 OR id = ANY(:ids))
          AND (cardinality(:accountIds) = 0 OR account_id = ANY(:accountIds))
        ORDER BY id
        LIMIT :page_size
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", query.SessionIds)
            .AddParameter("accountIds", query.AccountIds.Select(a => a.Value))
            .AddParameter("cursor", query.SessionIdCursor)
            .AddParameter("page_size", query.PageSize);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            if (reader.IsDBNull(reader.GetOrdinal("account_id")))
            {
                yield return new AdminSession(
                    reader.GetGuid("id"),
                    reader.GetFieldValue<DateTimeOffset>("created_at"));

                continue;
            }

            yield return new UserSession(
                reader.GetGuid("id"),
                new AccountId(reader.GetInt64("account_id")),
                reader.GetFieldValue<DateTimeOffset>("created_at"));
        }
    }

    public async Task<SessionBase?> FindSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        SessionQuery query = SessionSpecifications.ById(sessionId);

        return await QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }
}