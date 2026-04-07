namespace Bsa.Gateway.Presentation.Http.Requests.Invoices;

public readonly record struct RevokeInvoiceHttpRequest
{
    public required Guid SessionId { get; init; }

    public required long InvoiceId { get; init; }
}