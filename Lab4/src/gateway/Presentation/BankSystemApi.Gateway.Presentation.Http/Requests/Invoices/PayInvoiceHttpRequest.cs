namespace BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

public sealed class PayInvoiceHttpRequest
{
    public required long InvoiceId { get; init; }
}