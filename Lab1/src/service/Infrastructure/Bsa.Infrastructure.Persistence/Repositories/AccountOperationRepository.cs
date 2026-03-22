using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Domain.Accounts;
using Bsa.Domain.Operations;
using Bsa.Domain.ValueObjects;
using Npgsql;
using System.Runtime.CompilerServices;
using AccountOperationQuery = Bsa.Application.Abstractions.Persistence.Queries.AccountOperationQuery;

namespace Bsa.Infrastructure.Persistence.Repositories;

public sealed class AccountOperationRepository : IAccountOperationRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public AccountOperationRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async IAsyncEnumerable<AccountOperation> AddAsync(
        IReadOnlyCollection<AccountOperation> accountOperations,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO account_operations (account_id, account_number, balance, operation_type, created_at)
        SELECT acc_id, acc_num, bal, op_type, op_time
        FROM unnest(:accIds, :nums, :bals, :types, :times) AS source(acc_id, acc_num, bal, op_type, op_time)
        RETURNING id, account_id, account_number, balance, operation_type, created_at
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("accIds", accountOperations.Select(o => o.AccountId.Value).ToArray()),
                new NpgsqlParameter("nums", accountOperations.Select(o => o.Number.Value).ToArray()),
                new NpgsqlParameter("bals", accountOperations.Select(o => o.Balance.Value).ToArray()),
                new NpgsqlParameter("types", accountOperations.Select(o => o.OperationType.ToString()).ToArray()),
                new NpgsqlParameter("times", accountOperations.Select(o => o.CreatedTime).ToArray()),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int idIndex = reader.GetOrdinal("id");
        int accountIdIndex = reader.GetOrdinal("account_id");
        int numberIndex = reader.GetOrdinal("account_number");
        int balanceIndex = reader.GetOrdinal("balance");
        int typeIndex = reader.GetOrdinal("operation_type");
        int timeIndex = reader.GetOrdinal("created_at");

        while (await reader.ReadAsync(cancellationToken))
        {
            DateTimeOffset createdTime = await reader.GetFieldValueAsync<DateTimeOffset>(timeIndex, cancellationToken);

            yield return new AccountOperation(
                new AccountOperationId(reader.GetInt64(idIndex)),
                new AccountId(reader.GetInt64(accountIdIndex)),
                new AccountNumber(reader.GetString(numberIndex)),
                new Money(reader.GetDecimal(balanceIndex)),
                Enum.Parse<AccountOperationType>(reader.GetString(typeIndex)),
                createdTime);
        }
    }

    public async IAsyncEnumerable<AccountOperation> QueryAsync(
        AccountOperationQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, account_id, account_number, balance, operation_type, created_at
        FROM account_operations
        WHERE 
           (:cursor IS NULL OR id > :cursor)
           AND (cardinality(:accountIds) = 0 OR account_id = ANY(:accountIds))
        ORDER BY id
        LIMIT :page_size
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("accountIds", query.AccountIds.Select(i => i.Value).ToArray()),
                new NpgsqlParameter("page_size", query.PageSize),
                new NpgsqlParameter<long?>("cursor", query.IdCursor?.Value),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        int idIndex = reader.GetOrdinal("id");
        int accountInIndex = reader.GetOrdinal("account_id");
        int numberIndex = reader.GetOrdinal("account_number");
        int balanceIndex = reader.GetOrdinal("balance");
        int typeIndex = reader.GetOrdinal("operation_type");
        int timeIndex = reader.GetOrdinal("created_at");

        while (await reader.ReadAsync(cancellationToken))
        {
            DateTimeOffset createdTime = await reader.GetFieldValueAsync<DateTimeOffset>(timeIndex, cancellationToken);

            yield return new AccountOperation(
                new AccountOperationId(reader.GetInt64(idIndex)),
                new AccountId(reader.GetInt64(accountInIndex)),
                new AccountNumber(reader.GetString(numberIndex)),
                new Money(reader.GetDecimal(balanceIndex)),
                Enum.Parse<AccountOperationType>(reader.GetString(typeIndex)),
                createdTime);
        }
    }
}