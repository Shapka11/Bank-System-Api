using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Domain.Invoices;

namespace Bsa.Infrastructure.Persistence.Specifications;

public static class InvoiceSpecifications
{
    public static InvoiceQuery ById(InvoiceId id)
    {
        return InvoiceQuery.Build(builder => builder
            .WithId(id)
            .WithPageSize(1));
    }
}