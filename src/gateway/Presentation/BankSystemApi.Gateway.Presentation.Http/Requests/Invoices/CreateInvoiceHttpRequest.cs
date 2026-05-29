using BankSystemApi.Gateway.Presentation.Http.Attributes;

namespace BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

public sealed class CreateInvoiceHttpRequest
{
    [NotWhiteSpace]
    public required long SenderAccountId { get; init; }

    [NotWhiteSpace]
    public required long ReceiverAccountId { get; init; }

    public required decimal Amount { get; init; }
}