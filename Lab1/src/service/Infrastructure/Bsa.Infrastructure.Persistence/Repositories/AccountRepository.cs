using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Domain.Accounts;
using Bsa.Domain.ValueObjects;
using Npgsql;
using System.Runtime.CompilerServices;
using AccountQuery = Bsa.Application.Abstractions.Persistence.Queries.AccountQuery;

namespace Bsa.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public AccountRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async IAsyncEnumerable<Account> AddAsync(
        IReadOnlyCollection<Account> accounts,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO accounts (account_number, password, balance, created_at, updated_at)
        SELECT number, password, balance, created_at, updated_at
        FROM unnest(:numbers, :passwords, :balances, :createdAts, :updatedAts) 
           AS source(number, password, balance, created_at, updated_at)
        RETURNING id, account_number, password, balance, created_at, updated_at
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("numbers", accounts.Select(a => a.Number.Value).ToArray()),
                new NpgsqlParameter("passwords", accounts.Select(a => a.Password.Value).ToArray()),
                new NpgsqlParameter("balances", accounts.Select(a => a.Balance.Value).ToArray()),
                new NpgsqlParameter("createdAts", accounts.Select(a => a.CreatedTime).ToArray()),
                new NpgsqlParameter("updatedAts", accounts.Select(a => a.UpdatedTime).ToArray()),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int idIndex = reader.GetOrdinal("id");
        int accountNumberIndex = reader.GetOrdinal("account_number");
        int passwordIndex = reader.GetOrdinal("password");
        int balanceIndex = reader.GetOrdinal("balance");
        int createdTimeIndex = reader.GetOrdinal("created_at");
        int updatedTimeIndex = reader.GetOrdinal("updated_at");

        while (await reader.ReadAsync(cancellationToken))
        {
            DateTimeOffset createdTime =
                await reader.GetFieldValueAsync<DateTimeOffset>(createdTimeIndex, cancellationToken);
            DateTimeOffset updatedTime =
                await reader.GetFieldValueAsync<DateTimeOffset>(updatedTimeIndex, cancellationToken);

            yield return new Account(
                new AccountId(reader.GetInt64(idIndex)),
                new AccountNumber(reader.GetString(accountNumberIndex)),
                new Password(reader.GetString(passwordIndex)),
                new Money(reader.GetDecimal(balanceIndex)),
                createdTime,
                updatedTime);
        }
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

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("ids", accounts.Select(a => a.Id.Value).ToArray()),
                new NpgsqlParameter("numbers", accounts.Select(a => a.Number.Value).ToArray()),
                new NpgsqlParameter("passwords", accounts.Select(a => a.Password.Value).ToArray()),
                new NpgsqlParameter("balances", accounts.Select(a => a.Balance.Value).ToArray()),
                new NpgsqlParameter("updatedAts", accounts.Select(a => a.UpdatedTime).ToArray()),
            },
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<Account> QueryAsync(
        AccountQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, account_number, password, balance, created_at, updated_at
        FROM accounts
        WHERE
           (:cursor IS NULL OR id > :cursor)
           AND (cardinality(:numbers) = 0 OR account_number = ANY(:numbers))
           AND (cardinality(:ids) = 0 OR id = ANY(:ids))
        ORDER BY id
        LIMIT :page_size
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("ids", query.AccountIds.Select(i => i.Value).ToArray()),
                new NpgsqlParameter("numbers", query.AccountNumbers.Select(a => a.Value).ToArray()),
                new NpgsqlParameter<long?>("cursor", query.AccountIdCursor?.Value),
                new NpgsqlParameter("page_size", query.PageSize),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int accountIdIndex = reader.GetOrdinal("id");
        int accountNumberIndex = reader.GetOrdinal("account_number");
        int passwordIndex = reader.GetOrdinal("password");
        int balanceIndex = reader.GetOrdinal("balance");
        int createdTimeIndex = reader.GetOrdinal("created_at");
        int updatedTimeIndex = reader.GetOrdinal("updated_at");

        while (await reader.ReadAsync(cancellationToken))
        {
            DateTimeOffset createdTime =
                await reader.GetFieldValueAsync<DateTimeOffset>(createdTimeIndex, cancellationToken);
            DateTimeOffset updatedTime =
                await reader.GetFieldValueAsync<DateTimeOffset>(updatedTimeIndex, cancellationToken);

            yield return new Account(
                new AccountId(reader.GetInt64(accountIdIndex)),
                new AccountNumber(reader.GetString(accountNumberIndex)),
                new Password(reader.GetString(passwordIndex)),
                new Money(reader.GetDecimal(balanceIndex)),
                createdTime,
                updatedTime);
        }
    }

    public async Task<Account?> FindAccountByNumberAsync(AccountNumber number, CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = AccountQuery.Build(builder => builder
            .WithAccountNumber(number)
            .WithPageSize(pageSize));

        return await QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<Account?> FindAccountByIdAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = AccountQuery.Build(builder => builder
            .WithAccountId(accountId)
            .WithPageSize(pageSize));

        return await QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }
}