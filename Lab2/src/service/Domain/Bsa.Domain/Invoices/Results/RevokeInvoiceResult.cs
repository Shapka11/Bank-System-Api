namespace Bsa.Domain.Invoices.Results;

public abstract record RevokeInvoiceResult
{
    private RevokeInvoiceResult() { }

    public sealed record Success : RevokeInvoiceResult;

    public sealed record Failure(string ErrorMessage) : RevokeInvoiceResult;
}