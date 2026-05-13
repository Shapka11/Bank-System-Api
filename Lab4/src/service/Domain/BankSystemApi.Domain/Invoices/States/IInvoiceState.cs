namespace BankSystemApi.Domain.Invoices.States;

public interface IInvoiceState
{
    InvoiceStatus State { get; }

    bool CanPay();

    bool CanRevoke();
}