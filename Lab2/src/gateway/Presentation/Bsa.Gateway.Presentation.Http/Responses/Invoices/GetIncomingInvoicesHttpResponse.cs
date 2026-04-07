using Bsa.Gateway.Presentation.Http.Models.Invoices;

namespace Bsa.Gateway.Presentation.Http.Responses.Invoices;

public readonly record struct GetIncomingInvoicesHttpResponse
{
    public required IEnumerable<InvoiceModel> Invoices { get; init; }

    public string? PageToken { get; init; }
}