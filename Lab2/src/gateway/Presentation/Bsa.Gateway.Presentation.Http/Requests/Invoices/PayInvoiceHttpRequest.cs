namespace Bsa.Gateway.Presentation.Http.Requests.Invoices;

public readonly record struct PayInvoiceHttpRequest
{
    public required Guid SessionId { get; init; }

    public required long InvoiceId { get; init; }
}