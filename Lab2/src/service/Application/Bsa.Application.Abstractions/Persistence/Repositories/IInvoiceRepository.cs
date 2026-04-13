using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Domain.Invoices;

namespace Bsa.Application.Abstractions.Persistence.Repositories;

public interface IInvoiceRepository
{
    IAsyncEnumerable<Invoice> AddAsync(IReadOnlyCollection<Invoice> invoices, CancellationToken cancellationToken);

    Task UpdateAsync(IReadOnlyCollection<Invoice> invoices, CancellationToken cancellationToken);

    IAsyncEnumerable<Invoice> QueryAsync(InvoiceQuery query, CancellationToken cancellationToken);
}