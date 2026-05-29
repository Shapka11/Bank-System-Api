namespace BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

public sealed class ApproveInvoiceHttpRequest
{
    public required long InvoiceId { get; init; }
}