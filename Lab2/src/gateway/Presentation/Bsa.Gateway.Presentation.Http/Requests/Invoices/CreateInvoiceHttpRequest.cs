using Bsa.Gateway.Presentation.Http.Attributes;

namespace Bsa.Gateway.Presentation.Http.Requests.Invoices;

public sealed class CreateInvoiceHttpRequest
{
    public required Guid SessionId { get; init; }

    [NotWhiteSpace]
    public required string SenderAccountNumber { get; init; }

    [NotWhiteSpace]
    public required string ReceiverAccountNumber { get; init; }

    public required decimal Amount { get; init; }
}