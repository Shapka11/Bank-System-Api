using Bsa.Gateway.Presentation.Http.Models.Invoices;

namespace Bsa.Gateway.Presentation.Http.Responses.Invoices;

public readonly record struct CreateInvoiceHttpResponse
{
    public required InvoiceModel Invoice { get; init; }
}