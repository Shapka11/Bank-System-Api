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

internal sealed class AccountRepository : IAccountRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public AccountRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async IAsyncEnumerable<Account> AddAsync(
        IReadOnlyCollection<Account> accounts,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO accounts (user_id, type, account_number, password, balance, created_at, updated_at)
        SELECT user_id, type, number, password, balance, created_at, updated_at
        FROM unnest(:userids, :types, :numbers, :passwords, :balances, :createdAts, :updatedAts) 
           AS source(user_id, type, number, password, balance, created_at, updated_at)
        RETURNING account_id, user_id, type, account_number, password, balance, created_at, updated_at
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("userIds", accounts.Select(a => a.UserId.Value))
            .AddParameter("types", accounts.Select(a => a.Type))
            .AddParameter("numbers", accounts.Select(a => a.Number.Value))
            .AddParameter("passwords", accounts.Select(a => a.Password.Value))
            .AddParameter("balances", accounts.Select(a => a.Balance.Value))
            .AddParameter("createdAts", accounts.Select(a => a.CreatedAt))
            .AddParameter("updatedAts", accounts.Select(a => a.UpdatedAt));

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return CreateAccount(reader);
        }
    }

    public async Task UpdateAsync(IReadOnlyCollection<Account> accounts, CancellationToken cancellationToken)
    {
        const string sql = """
        UPDATE accounts
        SET password = source.password,
            balance = source.balance,
            type = source.type,
            updated_at = source.updated_at
        FROM unnest (:ids, :types, :numbers, :passwords, :balances, :updatedAts)
           AS source(id, type, number, password, balance, updated_at)
        WHERE accounts.account_id = source.id
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", accounts.Select(a => a.Id.Value))
            .AddParameter("types", accounts.Select(a => a.Type))
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
        SELECT account_id, user_id, type, account_number, password, balance, created_at, updated_at
        FROM accounts
        WHERE
           (:cursor IS NULL OR account_id > :cursor)
           AND (cardinality(:numbers) = 0 OR account_number = ANY(:numbers))
           AND (cardinality(:ids) = 0 OR account_id = ANY(:ids))
           AND (cardinality(:userIds) = 0 OR user_id = ANY(:userIds))
        ORDER BY account_id
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
            yield return CreateAccount(reader);
        }
    }

    private static Account CreateAccount(DbDataReader reader)
    {
        return new Account(
            new AccountId(reader.GetInt64("account_id")),
            new UserId(reader.GetInt64("user_id")),
            reader.GetFieldValue<AccountType>("type"),
            new AccountNumber(reader.GetString("account_number")),
            new Password(reader.GetString("password")),
            new Money(reader.GetDecimal("balance")),
            reader.GetFieldValue<DateTimeOffset>("created_at"),
            reader.GetFieldValue<DateTimeOffset>("updated_at"));
    }
}