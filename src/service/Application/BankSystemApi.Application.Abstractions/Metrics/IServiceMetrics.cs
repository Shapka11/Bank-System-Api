namespace BankSystemApi.Application.Abstractions.Metrics;

public interface IServiceMetrics
{
    void IncAccountCreated();

    void IncAccountDeposit();

    void IncAccountWithdrawal();

    void IncInvoiceCreated();

    void IncInvoicePaid();

    void IncInvoiceRevoked();

    void IncInvoiceApproved();

    void InvInvoiceDeclined();
}