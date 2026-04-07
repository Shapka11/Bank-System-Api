using Bsa.Gateway.Application.Contracts.Invoices.Models;

namespace Bsa.Gateway.Application.Contracts.Invoices.Operations.Responses;

public abstract record CreateInvoiceResponse
{
    private CreateInvoiceResponse() { }

    public sealed record Success(InvoiceDto Invoice) : CreateInvoiceResponse;

    public sealed record Failure(string ErrorMessage) : CreateInvoiceResponse;
}