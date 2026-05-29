namespace BankSystemApi.Domain.Invoices.Results;

public abstract record ApproveInvoiceResult
{
    private ApproveInvoiceResult() { }

    public sealed record Success() : ApproveInvoiceResult;

    public sealed record Failure(string ErrorMessage) : ApproveInvoiceResult;
}