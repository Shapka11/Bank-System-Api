namespace Bsa.Gateway.Presentation.Http.Requests.Invoices;

public sealed class PayInvoiceHttpRequest
{
    public required Guid SessionId { get; init; }

    public required long InvoiceId { get; init; }
}