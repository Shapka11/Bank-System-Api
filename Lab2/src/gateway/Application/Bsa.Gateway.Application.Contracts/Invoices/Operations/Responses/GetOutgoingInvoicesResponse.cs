using Bsa.Gateway.Application.Contracts.Invoices.Models;

namespace Bsa.Gateway.Application.Contracts.Invoices.Operations.Responses;

public abstract record GetOutgoingInvoicesResponse
{
    private GetOutgoingInvoicesResponse() { }

    public sealed record Success(IEnumerable<InvoiceDto> Invoices, string? PageToken) : GetOutgoingInvoicesResponse;

    public sealed record Failure(string ErrorMessage) : GetOutgoingInvoicesResponse;
}