using BankSystemApi.Gateway.Presentation.Http.Models.Invoices;

namespace BankSystemApi.Gateway.Presentation.Http.Responses.Invoices;

public readonly record struct CreateInvoiceHttpResponse
{
    public required InvoiceModel Invoice { get; init; }
}