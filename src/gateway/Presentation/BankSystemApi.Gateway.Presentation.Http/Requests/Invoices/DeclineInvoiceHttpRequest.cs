namespace BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

public sealed class DeclineInvoiceHttpRequest
{
    public required long InvoiceId { get; init; }
}