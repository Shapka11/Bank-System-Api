using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Application.Abstractions.Persistence.Results;
using BankSystemApi.Domain.Users;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using UserQuery = BankSystemApi.Application.Abstractions.Persistence.Queries.UserQuery;

namespace BankSystemApi.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public UserRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<AddUserResult> TryAddAsync(IReadOnlyCollection<User> users, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO users (authorization_id, created_at)
        SELECT authorization_id, created_at
        FROM unnest(:authorizationIds, :createdAts) 
            AS source(authorization_id, created_at)
        ON CONFLICT (authorization_id) DO NOTHING
        RETURNING id, authorization_id, created_at
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("authorizationIds", users.Select(u => u.AuthorizationId))
            .AddParameter("createdAts", users.Select(u => u.CreatedAt));

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            var insertedUser = new User(
                new UserId(reader.GetInt64("id")),
                reader.GetGuid("authorization_id"),
                reader.GetFieldValue<DateTimeOffset>("created_at"));

            return new AddUserResult.Success(insertedUser);
        }

        return new AddUserResult.AlreadyExist();
    }

    public async IAsyncEnumerable<User> QueryAsync(
        UserQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, authorization_id, created_at
        FROM users
        WHERE
          (:cursor IS NULL OR id > :cursor)
          AND (cardinality(:ids) = 0 OR id = ANY(:ids))
          AND (cardinality(:authorizationIds) = 0 OR authorization_id = ANY(:authorizationIds))
        ORDER BY id
        LIMIT :page_size
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", query.Ids.Select(i => i.Value))
            .AddParameter("authorizationIds", query.AuthorizationIds)
            .AddParameter("cursor", query.SessionIdCursor?.Value)
            .AddParameter("page_size", query.PageSize);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new User(
                new UserId(reader.GetInt64("id")),
                reader.GetGuid("authorization_id"),
                reader.GetFieldValue<DateTimeOffset>("created_at"));
        }
    }
}