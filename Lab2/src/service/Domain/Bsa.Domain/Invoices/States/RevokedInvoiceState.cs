namespace Bsa.Domain.Invoices.States;

public sealed class RevokedInvoiceState : IInvoiceState
{
    public InvoiceStatus State => InvoiceStatus.Revoked;

    public bool CanPay() => false;

    public bool CanRevoke() => false;
}