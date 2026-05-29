namespace BankSystemApi.Domain.Invoices.States;

public sealed class DeclinedInvoiceState : IInvoiceState
{
    public InvoiceStatus State => InvoiceStatus.Declined;

    public bool CanPay() => false;

    public bool CanRevoke() => false;

    public bool CanApprove() => false;

    public bool CanDecline() => false;
}