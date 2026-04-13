using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Domain.Invoices;

namespace Bsa.Application.Specifications;

public sealed class InvoiceSpecifications
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceSpecifications(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Invoice?> FindById(InvoiceId id, CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = InvoiceQuery.Build(builder => builder
            .WithId(id)
            .WithPageSize(pageSize));

        return await _invoiceRepository.QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }
}