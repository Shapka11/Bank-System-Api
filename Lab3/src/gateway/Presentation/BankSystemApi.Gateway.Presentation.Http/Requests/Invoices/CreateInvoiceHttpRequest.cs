using BankSystemApi.Gateway.Presentation.Http.Attributes;

namespace BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

public sealed class CreateInvoiceHttpRequest
{
    [NotWhiteSpace]
    public required Guid SenderAccountId { get; init; }

    [NotWhiteSpace]
    public required Guid ReceiverAccountId { get; init; }

    public required decimal Amount { get; init; }
}