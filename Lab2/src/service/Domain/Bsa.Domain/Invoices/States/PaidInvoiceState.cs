namespace Bsa.Domain.Invoices.States;

public sealed class PaidInvoiceState : IInvoiceState
{
    public InvoiceStatus State => InvoiceStatus.Paid;

    public bool CanPay() => false;

    public bool CanRevoke() => false;
}