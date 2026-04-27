using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using AccountQuery = BankSystemApi.Application.Abstractions.Persistence.Queries.AccountQuery;

namespace BankSystemApi.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public AccountRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task AddAsync(IReadOnlyCollection<Account> accounts, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO accounts (id, user_id, account_number, password, balance, created_at, updated_at)
        SELECT id, user_id, number, password, balance, created_at, updated_at
        FROM unnest(:ids, :userids, :numbers, :passwords, :balances, :createdAts, :updatedAts) 
           AS source(id, user_id, number, password, balance, created_at, updated_at)
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", accounts.Select(a => a.Id.Value))
            .AddParameter("userIds", accounts.Select(a => a.UserId.Value))
            .AddParameter("numbers", accounts.Select(a => a.Number.Value))
            .AddParameter("passwords", accounts.Select(a => a.Password.Value))
            .AddParameter("balances", accounts.Select(a => a.Balance.Value))
            .AddParameter("createdAts", accounts.Select(a => a.CreatedAt))
            .AddParameter("updatedAts", accounts.Select(a => a.UpdatedAt));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task UpdateAsync(IReadOnlyCollection<Account> accounts, CancellationToken cancellationToken)
    {
        const string sql = """
        UPDATE accounts
        SET password = source.password,
            balance = source.balance,
            updated_at = source.updated_at
        FROM unnest (:ids, :numbers, :passwords, :balances, :updatedAts)
           AS source(id, number, password, balance, updated_at)
        WHERE accounts.id = source.id
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", accounts.Select(a => a.Id.Value))
            .AddParameter("numbers", accounts.Select(a => a.Number.Value))
            .AddParameter("passwords", accounts.Select(a => a.Password.Value))
            .AddParameter("balances", accounts.Select(a => a.Balance.Value))
            .AddParameter("updatedAts", accounts.Select(a => a.UpdatedAt));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<Account> QueryAsync(
        AccountQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, user_id, account_number, password, balance, created_at, updated_at
        FROM accounts
        WHERE
           (:cursor IS NULL OR id > :cursor)
           AND (cardinality(:numbers) = 0 OR account_number = ANY(:numbers))
           AND (cardinality(:ids) = 0 OR id = ANY(:ids))
           AND (cardinality(:userIds) = 0 OR user_id = ANY(:userIds))
        ORDER BY id
        LIMIT :page_size
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", query.AccountIds.Select(i => i.Value))
            .AddParameter("userIds", query.UserIds.Select(i => i.Value))
            .AddParameter("numbers", query.AccountNumbers.Select(a => a.Value))
            .AddParameter("cursor", query.AccountIdCursor?.Value)
            .AddParameter("page_size", query.PageSize);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Account(
                new AccountId(reader.GetGuid("id")),
                new UserId(reader.GetInt64("user_id")),
                new AccountNumber(reader.GetString("account_number")),
                new Password(reader.GetString("password")),
                new Money(reader.GetDecimal("balance")),
                reader.GetFieldValue<DateTimeOffset>("created_at"),
                reader.GetFieldValue<DateTimeOffset>("updated_at"));
        }
    }
}