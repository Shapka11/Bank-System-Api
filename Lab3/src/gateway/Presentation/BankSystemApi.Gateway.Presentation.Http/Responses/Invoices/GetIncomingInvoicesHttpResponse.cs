using BankSystemApi.Gateway.Presentation.Http.Models.Invoices;

namespace BankSystemApi.Gateway.Presentation.Http.Responses.Invoices;

public readonly record struct GetIncomingInvoicesHttpResponse
{
    public required IReadOnlyCollection<InvoiceModel> Invoices { get; init; }

    public string? PageToken { get; init; }
}