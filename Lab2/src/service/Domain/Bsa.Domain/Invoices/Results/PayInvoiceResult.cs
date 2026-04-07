namespace Bsa.Domain.Invoices.Results;

public abstract record PayInvoiceResult
{
    private PayInvoiceResult() { }

    public sealed record Success : PayInvoiceResult;

    public sealed record Failure(string ErrorMessage) : PayInvoiceResult;
}