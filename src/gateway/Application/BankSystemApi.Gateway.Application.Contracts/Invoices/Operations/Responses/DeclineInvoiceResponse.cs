namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Responses;

public abstract record DeclineInvoiceResponse
{
    private DeclineInvoiceResponse() { }

    public sealed record Success() : DeclineInvoiceResponse;

    public sealed record Failure(string ErrorMessage) : DeclineInvoiceResponse;
}