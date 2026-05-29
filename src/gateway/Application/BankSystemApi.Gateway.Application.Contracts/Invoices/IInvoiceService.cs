using BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;
using BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Responses;

namespace BankSystemApi.Gateway.Application.Contracts.Invoices;

public interface IInvoiceService
{
    Task<CreateInvoiceResponse> CreateAsync(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken);

    Task<PayInvoiceResponse> PayAsync(PayInvoiceRequest request, CancellationToken cancellationToken);

    Task<RevokeInvoiceResponse> RevokeAsync(
        RevokeInvoiceRequest request,
        CancellationToken cancellationToken);

    Task<ApproveInvoiceResponse> ApproveAsync(
        ApproveInvoiceRequest request,
        CancellationToken cancellationToken);

    Task<DeclineInvoiceResponse> DeclineAsync(
        DeclineInvoiceRequest request,
        CancellationToken cancellationToken);

    Task<AssignAccountantResponse> AssignAccountantAsync(
        AssignAccountantRequest request,
        CancellationToken cancellationToken);

    Task<GetOutgoingInvoicesResponse> GetOutgoingAsync(
        GetOutgoingInvoicesRequest request,
        CancellationToken cancellationToken);

    Task<GetIncomingInvoicesResponse> GetIncomingAsync(
        GetIncomingInvoicesRequest request,
        CancellationToken cancellationToken);
}