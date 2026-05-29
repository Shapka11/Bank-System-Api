namespace BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;

public sealed class AssignAccountantHttpRequest
{
    public required long InvoiceId { get; init; }

    public required long AccountantId { get; init; }
}