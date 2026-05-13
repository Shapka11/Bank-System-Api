using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Domain.Invoices;

namespace BankSystemApi.Application.Abstractions.Persistence.Repositories;

public interface IInvoiceRepository
{
    IAsyncEnumerable<Invoice> AddAsync(IReadOnlyCollection<Invoice> invoices, CancellationToken cancellationToken);

    Task UpdateAsync(IReadOnlyCollection<Invoice> invoices, CancellationToken cancellationToken);

    IAsyncEnumerable<Invoice> QueryAsync(InvoiceQuery query, CancellationToken cancellationToken);
}