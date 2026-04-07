namespace Bsa.Gateway.Presentation.Http.Requests.Invoices;

public readonly record struct CreateInvoiceHttpRequest
{
    public required Guid SessionId { get; init; }

    public required string SenderAccountNumber { get; init; }

    public required string ReceiverAccountNumber { get; init; }

    public required decimal Amount { get; init; }
}