using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Domain.Invoices;
using InvoiceQuery = BankSystemApi.Application.Abstractions.Persistence.Queries.InvoiceQuery;

namespace BankSystemApi.Application.Specifications;

public static class InvoiceSpecifications
{
    public static ValueTask<Invoice?> FindById(
        this IInvoiceRepository repository,
        InvoiceId id,
        CancellationToken cancellationToken)
    {
        const int pageSize = 1;
        var query = InvoiceQuery.Build(builder => builder
            .WithId(id)
            .WithPageSize(pageSize));

        return repository.QueryAsync(query, cancellationToken).SingleOrDefaultAsync(cancellationToken);
    }
}