namespace BankSystemApi.Domain.Invoices.States;

public sealed class CreatedInvoiceState : IInvoiceState
{
    public InvoiceStatus State => InvoiceStatus.Created;

    public bool CanPay() => false;

    public bool CanRevoke() => true;

    public bool CanApprove() => true;

    public bool CanDecline() => true;
}