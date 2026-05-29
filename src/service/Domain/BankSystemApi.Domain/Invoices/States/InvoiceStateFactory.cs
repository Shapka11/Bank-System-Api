namespace BankSystemApi.Domain.Invoices.States;

public static class InvoiceStateFactory
{
    public static IInvoiceState Create(InvoiceStatus status)
    {
        return status switch
        {
            InvoiceStatus.Created => new CreatedInvoiceState(),
            InvoiceStatus.Paid => new PaidInvoiceState(),
            InvoiceStatus.Revoked => new RevokedInvoiceState(),
            InvoiceStatus.Approved => new ApprovedInvoiceState(),
            InvoiceStatus.Declined => new DeclinedInvoiceState(),
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unknow state: {status}"),
        };
    }
}