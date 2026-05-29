using BankSystemApi.Application.Abstractions.Metrics;
using Prometheus.Client;

namespace BankSystemApi.Metrics;

public sealed class PrometheusServiceMetrics(IMetricFactory metricFactory) : IServiceMetrics
{
    private readonly ICounter<long> _accountCreatedCounter = metricFactory.CreateCounterInt64(
        name: "bank_system_service_account_created_total",
        help: "Total number of created accounts");

    private readonly ICounter<long> _accountDepositCounter = metricFactory.CreateCounterInt64(
        name: "bank_system_service_account_deposit_total",
        help: "Total number of deposit accounts");

    private readonly ICounter<long> _accountWithdrawalCounter = metricFactory.CreateCounterInt64(
        name: "bank_system_service_account_withdrawal_total",
        help: "Total number of withdrawal accounts");

    private readonly ICounter<long> _invoiceCreatedCounter = metricFactory.CreateCounterInt64(
        name: "bank_system_service_invoice_created_total",
        help: "Total number of created invoices");

    private readonly ICounter<long> _invoicePaidCounter = metricFactory.CreateCounterInt64(
        name: "bank_system_service_invoice_paid_total",
        help: "Total number of paid invoices");

    private readonly ICounter<long> _invoiceRevokedCounter = metricFactory.CreateCounterInt64(
        name: "bank_system_service_invoice_revoked_total",
        help: "Total number of revoked invoices");

    private readonly ICounter<long> _invoiceApprovedCounter = metricFactory.CreateCounterInt64(
        name: "bank_system_service_invoice_approved_total",
        help: "Total number of approved invoices");

    private readonly ICounter<long> _invoiceDeclinedCounter = metricFactory.CreateCounterInt64(
        name: "bank_system_service_invoice_declined_total",
        help: "Total number of declined invoices");

    public void IncAccountCreated() => _accountCreatedCounter.Inc();

    public void IncAccountDeposit() => _accountDepositCounter.Inc();

    public void IncAccountWithdrawal() => _accountWithdrawalCounter.Inc();

    public void IncInvoiceCreated() => _invoiceCreatedCounter.Inc();

    public void IncInvoicePaid() => _invoicePaidCounter.Inc();

    public void IncInvoiceRevoked() => _invoiceRevokedCounter.Inc();

    public void IncInvoiceApproved() => _invoiceApprovedCounter.Inc();

    public void InvInvoiceDeclined() => _invoiceDeclinedCounter.Inc();
}