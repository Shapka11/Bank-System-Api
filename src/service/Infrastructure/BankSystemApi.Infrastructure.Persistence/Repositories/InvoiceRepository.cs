using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Domain.Invoices.States;
using BankSystemApi.Domain.ValueObjects;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using InvoiceQuery = BankSystemApi.Application.Abstractions.Persistence.Queries.InvoiceQuery;

namespace BankSystemApi.Infrastructure.Persistence.Repositories;

internal sealed class InvoiceRepository : IInvoiceRepository
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
        INSERT INTO invoices(sender_account_id,
                            receiver_account_id,
                            amount,
                            status,
                            created_at,
                            updated_at)
        SELECT sanderId, receiverId, amount, status, created_at, updated_at
        FROM unnest(:senderAccountIds, :receiverAccountIds, :amounts, :statuses, :createdAts, :updatedAts)
           AS source(sanderId, receiverId, amount, status, created_at, updated_at)
        RETURNING
           invoice_id,
           sender_account_id,
           receiver_account_id, 
           amount,
           status,
           created_at,
           updated_at
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("senderAccountIds", invoices.Select(i => i.SenderAccountId.Value))
            .AddParameter("receiverAccountIds", invoices.Select(i => i.ReceiverAccountId.Value))
            .AddParameter("amounts", invoices.Select(i => i.Amount.Value))
            .AddParameter("statuses", invoices.Select(i => i.State.State))
            .AddParameter("createdAts", invoices.Select(i => i.CreatedAt))
            .AddParameter("updatedAts", invoices.Select(i => i.UpdatedAt));

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return CreateInvoice(reader);
        }
    }

    public async Task UpdateAsync(IReadOnlyCollection<Invoice> invoices, CancellationToken cancellationToken)
    {
        const string sql = """
        UPDATE invoices
        SET sender_account_id = source.sender_account_ids,
           receiver_account_id = source.receiver_account_ids,
           amount = source.amount,
           status = source.status,
           updated_at = source.updated_at
        FROM unnest (:ids, :senderAccountIds, :receiverAccountIds, :amounts, :statuses, :updatedAts)
           AS source(id, sender_account_ids, receiver_account_ids, amount, status, updated_at)
        WHERE invoices.invoice_id = source.id
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", invoices.Select(i => i.Id.Value))
            .AddParameter("senderAccountIds", invoices.Select(i => i.SenderAccountId.Value))
            .AddParameter("receiverAccountIds", invoices.Select(i => i.ReceiverAccountId.Value))
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
        SELECT invoice_id,
              sender_account_id, 
              receiver_account_id,
              amount, 
              status, 
              created_at, 
              updated_at
        FROM invoices
        WHERE
           (:cursor IS NULL OR invoice_id > :cursor)
           AND (cardinality(:ids) = 0 OR invoice_id = ANY(:ids))
           AND (cardinality(:senderAccountIds) = 0 OR sender_account_id = ANY(:senderAccountIds))
           AND (cardinality(:receiverAccountIds) = 0 OR receiver_account_id = ANY(:receiverAccountIds)) 
           AND (cardinality(:statuses) = 0 OR status = ANY(:statuses))
        ORDER BY invoice_id
        LIMIT :page_size
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", query.Ids.Select(a => a.Value))
            .AddParameter("senderAccountIds", query.SenderAccountIds.Select(a => a.Value))
            .AddParameter("receiverAccountIds", query.ReceiverAccountIds.Select(a => a.Value))
            .AddParameter("statuses", query.Statuses)
            .AddParameter("cursor", query.InvoiceIdCursor?.Value)
            .AddParameter("page_size", query.PageSize);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return CreateInvoice(reader);
        }
    }

    private static Invoice CreateInvoice(DbDataReader reader)
    {
        return new Invoice(
            new InvoiceId(reader.GetInt64("invoice_id")),
            new AccountId(reader.GetInt64("sender_account_id")),
            new AccountId(reader.GetInt64("receiver_account_id")),
            new Money(reader.GetDecimal("amount")),
            InvoiceStateFactory.Create(reader.GetFieldValue<InvoiceStatus>("status")),
            reader.GetFieldValue<DateTimeOffset>("created_at"),
            reader.GetFieldValue<DateTimeOffset>("updated_at"));
    }
}