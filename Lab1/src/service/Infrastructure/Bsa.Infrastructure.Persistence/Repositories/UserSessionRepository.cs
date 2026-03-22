using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Domain.Accounts;
using Bsa.Domain.Sessions;
using Npgsql;
using System.Runtime.CompilerServices;

namespace Bsa.Infrastructure.Persistence.Repositories;

public sealed class UserSessionRepository : IUserSessionRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public UserSessionRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task AddAsync(IReadOnlyCollection<UserSession> sessions, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO user_sessions (id, account_id, created_at)
        SELECT id, account_id, created_at
        FROM unnest(:ids, :accountIds, :createdAts) AS source(id, account_id, created_at)
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("ids", sessions.Select(s => s.Id).ToArray()),
                new NpgsqlParameter("accountIds", sessions.Select(s => s.AccountId.Value).ToArray()),
                new NpgsqlParameter("createdAts", sessions.Select(s => s.CreatedTime).ToArray()),
            },
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task RemoveAsync(IReadOnlyCollection<UserSession> sessions, CancellationToken cancellationToken)
    {
        const string sql = """
        DELETE FROM user_sessions 
        WHERE id = ANY(:ids)
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("ids", sessions.Select(s => s.Id).ToArray()),
            },
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<UserSession> QueryAsync(
        UserSessionQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, account_id, created_at
        FROM user_sessions
        WHERE
          (:cursor IS NULL OR id > :cursor)
          AND (cardinality(:ids) = 0 OR id = ANY(:ids))
        ORDER BY id
        LIMIT :page_size
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("ids", query.SessionIds.ToArray()),
                new NpgsqlParameter<Guid?>("cursor", query.SessionIdCursor),
                new NpgsqlParameter("page_size", query.PageSize),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int idIndex = reader.GetOrdinal("id");
        int accountIdIndex = reader.GetOrdinal("account_id");
        int createdTimeIndex = reader.GetOrdinal("created_at");

        while (await reader.ReadAsync(cancellationToken))
        {
            DateTimeOffset createdTime =
                await reader.GetFieldValueAsync<DateTimeOffset>(createdTimeIndex, cancellationToken);

            yield return new UserSession(
                reader.GetGuid(idIndex),
                new AccountId(reader.GetInt64(accountIdIndex)),
                createdTime);
        }
    }

    public async Task<UserSession?> FindUserSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = UserSessionQuery.Build(builder => builder
            .WithSessionId(sessionId)
            .WithPageSize(pageSize));

        return await QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }
}