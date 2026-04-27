namespace BankSystemApi.Domain.Invoices;

public readonly record struct InvoiceId(long Value)
{
    public static InvoiceId Default => new(default);
}