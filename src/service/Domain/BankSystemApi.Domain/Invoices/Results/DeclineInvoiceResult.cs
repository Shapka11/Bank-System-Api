namespace BankSystemApi.Domain.Invoices.Results;

public abstract record DeclineInvoiceResult
{
    private DeclineInvoiceResult() { }

    public sealed record Success() : DeclineInvoiceResult;

    public sealed record Failure(string ErrorMessage) : DeclineInvoiceResult;
}