namespace BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

public sealed class RevokeInvoiceHttpRequest
{
    public required long InvoiceId { get; init; }
}