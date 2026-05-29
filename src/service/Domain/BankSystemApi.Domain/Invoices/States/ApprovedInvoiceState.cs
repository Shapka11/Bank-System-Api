namespace BankSystemApi.Domain.Invoices.States;

public sealed class ApprovedInvoiceState : IInvoiceState
{
    public InvoiceStatus State => InvoiceStatus.Approved;

    public bool CanPay() => true;

    public bool CanRevoke() => true;

    public bool CanApprove() => false;

    public bool CanDecline() => false;
}