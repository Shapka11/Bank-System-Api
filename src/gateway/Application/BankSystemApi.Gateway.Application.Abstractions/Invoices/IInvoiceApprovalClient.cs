using BankSystemApi.Gateway.Application.Abstractions.Invoices.Operations;

namespace BankSystemApi.Gateway.Application.Abstractions.Invoices;

public interface IInvoiceApprovalClient
{
    Task<ApproveInvoice.Response> ApproveAsync(
        ApproveInvoice.Request request,
        CancellationToken cancellationToken);

    Task<DeclineInvoice.Response> DeclineAsync(
        DeclineInvoice.Request request,
        CancellationToken cancellationToken);

    Task<AssignAccountant.Response> AssignAccountantAsync(
        AssignAccountant.Request request,
        CancellationToken cancellationToken);
}