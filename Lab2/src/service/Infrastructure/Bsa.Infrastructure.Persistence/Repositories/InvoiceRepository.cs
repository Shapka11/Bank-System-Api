using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Domain.Invoices;
using Bsa.Domain.Invoices.States;
using Bsa.Domain.ValueObjects;
using Bsa.Infrastructure.Persistence.Specifications;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Bsa.Infrastructure.Persistence.Repositories;

public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public InvoiceRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async IAsyncEnumerable<Invoice> AddAsync(
        IReadOnlyCollection<Invoice> invoices,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO invoices(sender_account_number,
                            receiver_account_number,
                            amount,
                            status,
                            created_at,
                            updated_at)
        SELECT sanderNumber, receiverNumber, amount, status, created_at, updated_at
        FROM unnest(:senderAccountNumbers, :receiverAccountNumbers, :amounts, :statuses, :createdAts, :updatedAts)
           AS source(sanderNumber, receiverNumber, amount, status, created_at, updated_at)
        RETURNING
           id,
           sender_account_number,
           receiver_account_number, 
           amount,
           status,
           created_at,
           updated_at
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("senderAccountNumbers", invoices.Select(i => i.SenderAccountNumber.Value))
            .AddParameter("receiverAccountNumbers", invoices.Select(i => i.ReceiverAccountNumber.Value))
            .AddParameter("amounts", invoices.Select(i => i.Amount.Value))
            .AddParameter("statuses", invoices.Select(i => i.State.State))
            .AddParameter("createdAts", invoices.Select(i => i.CreatedAt))
            .AddParameter("updatedAts", invoices.Select(i => i.UpdatedAt));

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            InvoiceStatus status = Enum.Parse<InvoiceStatus>(reader.GetString("status"));

            yield return new Invoice(
                new InvoiceId(reader.GetInt64("id")),
                new AccountNumber(reader.GetString("sender_account_number")),
                new AccountNumber(reader.GetString("receiver_account_number")),
                new Money(reader.GetDecimal("amount")),
                InvoiceStateFactory.Create(status),
                reader.GetFieldValue<DateTimeOffset>("created_at"),
                reader.GetFieldValue<DateTimeOffset>("updated_at"));
        }
    }

    public async Task UpdateAsync(IReadOnlyCollection<Invoice> invoices, CancellationToken cancellationToken)
    {
        const string sql = """
        UPDATE invoices
        SET sender_account_number = source.sender_account_numbers,
           receiver_account_number = source.receiver_account_numbers,
           amount = source.amount,
           status = source.status,
           updated_at = source.updated_at
        FROM unnest (:ids, :senderAccountNumbers, :receiverAccountNumbers, :amounts, :statuses, :updatedAts)
           AS source(id, sender_account_numbers, receiver_account_numbers, amount, status, updated_at)
        WHERE invoices.id = source.id
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", invoices.Select(i => i.Id.Value))
            .AddParameter("senderAccountNumbers", invoices.Select(i => i.SenderAccountNumber.Value))
            .AddParameter("receiverAccountNumbers", invoices.Select(i => i.ReceiverAccountNumber.Value))
            .AddParameter("amounts", invoices.Select(i => i.Amount.Value))
            .AddParameter("statuses", invoices.Select(i => i.State.State))
            .AddParameter("updatedAts", invoices.Select(i => i.UpdatedAt));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<Invoice> QueryAsync(
        InvoiceQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id,
              sender_account_number, 
              receiver_account_number,
              amount, 
              status, 
              created_at, 
              updated_at
        FROM invoices
        WHERE
           (:cursor IS NULL OR id > :cursor)
           AND (cardinality(:ids) = 0 OR id = ANY(:ids))
           AND (cardinality(:senderAccountNumbers) = 0 OR sender_account_number = ANY(:senderAccountNumbers))
           AND (cardinality(:receiverAccountNumbers) = 0 OR receiver_account_number = ANY(:receiverAccountNumbers)) 
           AND (cardinality(:statuses) = 0 OR status = ANY(:statuses))
        ORDER BY id
        LIMIT :page_size
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", query.Ids.Select(a => a.Value))
            .AddParameter("senderAccountNumbers", query.SenderAccountNumbers.Select(a => a.Value))
            .AddParameter("receiverAccountNumbers", query.ReceiverAccountNumbers.Select(a => a.Value))
            .AddParameter("statuses", query.Statuses)
            .AddParameter("cursor", query.InvoiceIdCursor?.Value)
            .AddParameter("page_size", query.PageSize);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            InvoiceStatus status = Enum.Parse<InvoiceStatus>(reader.GetString("status"));

            yield return new Invoice(
                new InvoiceId(reader.GetInt64("id")),
                new AccountNumber(reader.GetString("sender_account_number")),
                new AccountNumber(reader.GetString("receiver_account_number")),
                new Money(reader.GetDecimal("amount")),
                InvoiceStateFactory.Create(status),
                reader.GetFieldValue<DateTimeOffset>("created_at"),
                reader.GetFieldValue<DateTimeOffset>("updated_at"));
        }
    }

    public async Task<Invoice?> FindById(InvoiceId id, CancellationToken cancellationToken)
    {
        InvoiceQuery query = InvoiceSpecifications.ById(id);
        return await QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }
}