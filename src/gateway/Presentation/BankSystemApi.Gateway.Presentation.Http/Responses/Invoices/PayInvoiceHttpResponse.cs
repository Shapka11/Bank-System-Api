using BankSystemApi.Gateway.Presentation.Http.Models.Invoices;

namespace BankSystemApi.Gateway.Presentation.Http.Responses.Invoices;

public readonly record struct PayInvoiceHttpResponse
{
    public required InvoiceModel Invoice { get; init; }
}