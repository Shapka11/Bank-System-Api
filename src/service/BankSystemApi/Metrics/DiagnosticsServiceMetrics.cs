using BankSystemApi.Application.Abstractions.Metrics;
using System.Diagnostics.Metrics;

namespace BankSystemApi.Metrics;

public sealed class DiagnosticsServiceMetrics : IServiceMetrics
{
    public static Meter Meter => new("BankSystemApi.Service");

    private readonly Counter<long> _accountCreatedCounter = Meter
        .CreateCounter<long>("bank_system_service_account_created_total");

    private readonly Counter<long> _accountDepositCounter = Meter
        .CreateCounter<long>("bank_system_service_account_deposit_total");

    private readonly Counter<long> _accountWithdrawalCounter = Meter
        .CreateCounter<long>("bank_system_service_account_withdrawal_total");

    private readonly Counter<long> _invoiceCreatedCounter = Meter
        .CreateCounter<long>("bank_system_service_invoice_created_total");

    private readonly Counter<long> _invoicePaidCounter = Meter
        .CreateCounter<long>("bank_system_service_invoice_paid_total");

    private readonly Counter<long> _invoiceRevokedCounter = Meter
        .CreateCounter<long>("bank_system_service_invoice_revoked_total");

    private readonly Counter<long> _invoiceApprovedCounter = Meter
        .CreateCounter<long>("bank_system_service_invoice_approved_total");

    private readonly Counter<long> _invoiceDeclinedCounter = Meter
        .CreateCounter<long>("bank_system_service_invoice_declined_total");

    public void IncAccountCreated() => _accountCreatedCounter.Add(1);

    public void IncAccountDeposit() => _accountDepositCounter.Add(1);

    public void IncAccountWithdrawal() => _accountWithdrawalCounter.Add(1);

    public void IncInvoiceCreated() => _invoiceCreatedCounter.Add(1);

    public void IncInvoicePaid() => _invoicePaidCounter.Add(1);

    public void IncInvoiceRevoked() => _invoiceRevokedCounter.Add(1);

    public void IncInvoiceApproved() => _invoiceApprovedCounter.Add(1);

    public void InvInvoiceDeclined() => _invoiceDeclinedCounter.Add(1);
}