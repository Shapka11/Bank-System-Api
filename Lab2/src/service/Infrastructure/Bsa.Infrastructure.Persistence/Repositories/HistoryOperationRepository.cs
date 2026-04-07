using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Domain.Accounts;
using Bsa.Domain.HistoryOperations;
using Bsa.Domain.ValueObjects;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Factories;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Results;
using Bsa.Infrastructure.Persistence.Models;
using Bsa.Infrastructure.Persistence.Models.Payloads;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AccountOperationQuery = Bsa.Application.Abstractions.Persistence.Queries.AccountOperationQuery;

namespace Bsa.Infrastructure.Persistence.Repositories;

public sealed class HistoryOperationRepository : IHistoryOperationRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;
    private readonly HistoryOperationSerializationChainFactory _chainFactory;

    public HistoryOperationRepository(
        IPersistenceConnectionProvider connectionProvider,
        HistoryOperationSerializationChainFactory chainFactory)
    {
        _connectionProvider = connectionProvider;
        _chainFactory = chainFactory;
    }

    public async IAsyncEnumerable<HistoryOperation> AddAsync(
        IReadOnlyCollection<HistoryOperation> historyOperations,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO history_operations (account_id, account_number, occurred_at, payload)
        SELECT account_id, account_number, occurred_at, payload
        FROM unnest(:accountIds, :accountNumbers, :occurredAts, :payloads)
            AS source(account_id, account_number, occurred_at, payload)
        RETURNING id, account_id, account_number, occurred_at, payload
        """;

        IReadOnlyCollection<string> serializationPayloads = [];
        SerializationHistoryOperationResult serializationResult = _chainFactory.Create().Serialize(historyOperations);

        if (serializationResult is SerializationHistoryOperationResult.Failure failure)
            throw new InvalidOperationException(failure.ErrorMessage);
        if (serializationResult is SerializationHistoryOperationResult.Success success)
            serializationPayloads = success.PayloadJsons;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("accountIds", historyOperations.Select(o => o.AccountId.Value))
            .AddParameter("accountNumbers", historyOperations.Select(o => o.AccountNumber.Value))
            .AddParameter("occurredAts", historyOperations.Select(o => o.OccurredAt))
            .AddJsonArrayParameter("payloads", serializationPayloads);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return CreateHistoryOperation(reader);
        }
    }

    public async IAsyncEnumerable<HistoryOperation> QueryAsync(
        AccountOperationQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, account_id, account_number, occurred_at, payload
        FROM history_operations
        WHERE 
           (:cursor IS NULL OR id > :cursor)
           AND (cardinality(:accountIds) = 0 OR account_id = ANY(:accountIds))
        ORDER BY id
        LIMIT :page_size
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("accountIds", query.AccountIds.Select(i => i.Value))
            .AddParameter("page_size", query.PageSize)
            .AddParameter("cursor", query.IdCursor?.Value);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return CreateHistoryOperation(reader);
        }
    }

    private HistoryOperation CreateHistoryOperation(DbDataReader reader)
    {
        string payloadJson = reader.GetString(reader.GetOrdinal("payload"));
        PayloadBase? payload = JsonSerializer.Deserialize<PayloadBase>(payloadJson);

        var historyEntry = new HistoryOperationEntry(
            new HistoryOperationId(reader.GetInt64("id")),
            new AccountId(reader.GetInt64("account_id")),
            new AccountNumber(reader.GetString("account_number")),
            reader.GetFieldValue<DateTimeOffset>("occurred_at"),
            payload);

        DeserializationHistoryOperationResult deserializeResult =
            _chainFactory.Create().Deserialize(historyEntry);

        return deserializeResult switch
        {
            DeserializationHistoryOperationResult.Failure failure
                => throw new InvalidOperationException(failure.ErrorMessage),
            DeserializationHistoryOperationResult.Success success => success.Operation,
            _ => throw new UnreachableException(),
        };
    }
}