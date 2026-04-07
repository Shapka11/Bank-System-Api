using Bsa.Gateway.Application.Contracts.Invoices.Operations.Requests;
using Bsa.Gateway.Application.Contracts.Invoices.Operations.Responses;

namespace Bsa.Gateway.Application.Contracts.Invoices;

public interface IInvoiceService
{
    Task<CreateInvoiceResponse> CreateAsync(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken);

    Task<PayInvoiceResponse> PayAsync(PayInvoiceRequest request, CancellationToken cancellationToken);

    Task<RevokeInvoiceResponse> RevokeAsync(
        RevokeInvoiceRequest request,
        CancellationToken cancellationToken);

    Task<GetOutgoingInvoicesResponse> GetOutgoingAsync(
        GetOutgoingInvoicesRequest request,
        CancellationToken cancellationToken);

    Task<GetIncomingInvoicesResponse> GetIncomingAsync(
        GetIncomingInvoicesRequest request,
        CancellationToken cancellationToken);
}