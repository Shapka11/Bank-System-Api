namespace BankSystemApi.Domain.Invoices.States;

public sealed class CreatedInvoiceState : IInvoiceState
{
    public InvoiceStatus State => InvoiceStatus.Created;

    public bool CanPay() => true;

    public bool CanRevoke() => true;
}