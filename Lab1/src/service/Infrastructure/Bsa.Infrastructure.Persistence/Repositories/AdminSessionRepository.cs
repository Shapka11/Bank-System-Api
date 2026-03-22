using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Domain.Sessions;
using Npgsql;
using System.Runtime.CompilerServices;
using AdminSessionQuery = Bsa.Application.Abstractions.Persistence.Queries.AdminSessionQuery;

namespace Bsa.Infrastructure.Persistence.Repositories;

public sealed class AdminSessionRepository : IAdminSessionRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public AdminSessionRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task AddAsync(IReadOnlyCollection<AdminSession> sessions, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO admin_sessions (id, created_at)
        SELECT id, created_at
        FROM unnest(:ids, :createdAts) AS source(id, created_at)
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("ids", sessions.Select(s => s.Id).ToArray()),
                new NpgsqlParameter("createdAts", sessions.Select(s => s.CreatedTime).ToArray()),
            },
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task RemoveAsync(IReadOnlyCollection<AdminSession> sessions, CancellationToken cancellationToken)
    {
        const string sql = """
        DELETE FROM admin_sessions
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

    public async IAsyncEnumerable<AdminSession> QueryAsync(
        AdminSessionQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, created_at
        FROM admin_sessions
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
                new NpgsqlParameter("ids", query.Sessions.Select(s => s.Id).ToArray()),
                new NpgsqlParameter<Guid?>("cursor", query.SessionIdCursor),
                new NpgsqlParameter("page_size", query.PageSize),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int idIndex = reader.GetOrdinal("id");
        int createdTimeIndex = reader.GetOrdinal("created_at");

        while (await reader.ReadAsync(cancellationToken))
        {
            DateTimeOffset createdTime =
                await reader.GetFieldValueAsync<DateTimeOffset>(createdTimeIndex, cancellationToken);

            yield return new AdminSession(
                reader.GetGuid(idIndex),
                createdTime);
        }
    }

    public async Task<AdminSession?> FindAdminSessionAsync(
        AdminSession adminSession,
        CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = AdminSessionQuery.Build(builder => builder
            .WithSession(adminSession)
            .WithPageSize(pageSize));

        return await QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }
}