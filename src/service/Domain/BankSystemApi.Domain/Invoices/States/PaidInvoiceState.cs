namespace BankSystemApi.Domain.Invoices.States;

public sealed class PaidInvoiceState : IInvoiceState
{
    public InvoiceStatus State => InvoiceStatus.Paid;

    public bool CanPay() => false;

    public bool CanRevoke() => false;

    public bool CanApprove() => false;

    public bool CanDecline() => false;
}