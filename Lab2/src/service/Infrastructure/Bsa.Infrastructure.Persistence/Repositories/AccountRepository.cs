using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Domain.Accounts;
using Bsa.Domain.ValueObjects;
using Bsa.Infrastructure.Persistence.Specifications;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using AccountQuery = Bsa.Application.Abstractions.Persistence.Queries.AccountQuery;

namespace Bsa.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : IAccountRepository
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
        INSERT INTO accounts (account_number, password, balance, created_at, updated_at)
        SELECT number, password, balance, created_at, updated_at
        FROM unnest(:numbers, :passwords, :balances, :createdAts, :updatedAts) 
           AS source(number, password, balance, created_at, updated_at)
        RETURNING id, account_number, password, balance, created_at, updated_at
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("numbers", accounts.Select(a => a.Number.Value))
            .AddParameter("passwords", accounts.Select(a => a.Password.Value))
            .AddParameter("balances", accounts.Select(a => a.Balance.Value))
            .AddParameter("createdAts", accounts.Select(a => a.CreatedAt))
            .AddParameter("updatedAts", accounts.Select(a => a.UpdatedAt));

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Account(
                new AccountId(reader.GetInt64("id")),
                new AccountNumber(reader.GetString("account_number")),
                new Password(reader.GetString("password")),
                new Money(reader.GetDecimal("balance")),
                reader.GetFieldValue<DateTimeOffset>("created_at"),
                reader.GetFieldValue<DateTimeOffset>("updated_at"));
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
        SELECT id, account_number, password, balance, created_at, updated_at
        FROM accounts
        WHERE
           (:cursor IS NULL OR id > :cursor)
           AND (cardinality(:numbers) = 0 OR account_number = ANY(:numbers))
           AND (cardinality(:ids) = 0 OR id = ANY(:ids))
        ORDER BY id
        LIMIT :page_size
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", query.AccountIds.Select(i => i.Value))
            .AddParameter("numbers", query.AccountNumbers.Select(a => a.Value))
            .AddParameter("cursor", query.AccountIdCursor?.Value)
            .AddParameter("page_size", query.PageSize);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Account(
                new AccountId(reader.GetInt64("id")),
                new AccountNumber(reader.GetString("account_number")),
                new Password(reader.GetString("password")),
                new Money(reader.GetDecimal("balance")),
                reader.GetFieldValue<DateTimeOffset>("created_at"),
                reader.GetFieldValue<DateTimeOffset>("updated_at"));
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
        AccountQuery query = AccountSpecifications.ById(accountId);

        return await QueryAsync(query, cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public IAsyncEnumerable<Account> FindAccountByNumberAsync(
        IEnumerable<AccountNumber> accountNumbers,
        CancellationToken cancellationToken)
    {
        AccountQuery query = AccountSpecifications.ByNumbers(accountNumbers);

        return QueryAsync(query, cancellationToken);
    }
}