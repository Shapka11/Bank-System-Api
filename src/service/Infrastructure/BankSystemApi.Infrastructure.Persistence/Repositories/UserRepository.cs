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

    public async Task<AddUserResult> TryAddAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO users (authorization_id, created_at)
        VALUES (:authorizationIds, :createdAts)
        ON CONFLICT (authorization_id) DO NOTHING
        RETURNING user_id, authorization_id, created_at
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("authorizationIds", user.AuthorizationId)
            .AddParameter("createdAts", user.CreatedAt);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            User insertedUser = CreateUser(reader);

            return new AddUserResult.Success(insertedUser);
        }

        return new AddUserResult.AlreadyExist();
    }

    public async IAsyncEnumerable<User> QueryAsync(
        UserQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT user_id, authorization_id, created_at
        FROM users
        WHERE
          (:cursor IS NULL OR user_id > :cursor)
          AND (cardinality(:ids) = 0 OR user_id = ANY(:ids))
          AND (cardinality(:authorizationIds) = 0 OR authorization_id = ANY(:authorizationIds))
        ORDER BY user_id
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
            yield return CreateUser(reader);
        }
    }

    public async Task<User?> GetByIdForUpdateAsync(UserId userId, CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT user_id, authorization_id, created_at   
        FROM users
        WHERE user_id = :id
        FOR UPDATE
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("id", userId.Value);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            return CreateUser(reader);
        }

        return null;
    }

    private static User CreateUser(DbDataReader reader)
    {
        return new User(
            new UserId(reader.GetInt64("user_id")),
            reader.GetGuid("authorization_id"),
            reader.GetFieldValue<DateTimeOffset>("created_at"));
    }
}