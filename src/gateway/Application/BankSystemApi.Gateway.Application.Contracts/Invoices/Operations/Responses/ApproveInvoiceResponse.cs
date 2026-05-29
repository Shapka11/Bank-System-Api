namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Responses;

public abstract record ApproveInvoiceResponse
{
    private ApproveInvoiceResponse() { }

    public sealed record Success() : ApproveInvoiceResponse;

    public sealed record Failure(string ErrorMessage) : ApproveInvoiceResponse;
}