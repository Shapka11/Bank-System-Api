using Bsa.Application.Contracts.Invoices.Operations;

namespace Bsa.Application.Contracts.Invoices;

public interface IInvoiceService
{
    Task<CreateInvoice.Response> CreateAsync(
        CreateInvoice.Request request,
        CancellationToken cancellationToken);

    Task<PayInvoice.Response> PayAsync(
        PayInvoice.Request request,
        CancellationToken cancellationToken);

    Task<RevokeInvoice.Response> RevokeAsync(
        RevokeInvoice.Request request,
        CancellationToken cancellationToken);

    Task<GetIncomingInvoices.Response> GetIncomingAsync(
        GetIncomingInvoices.Request request,
        CancellationToken cancellationToken);

    Task<GetOutgoingInvoices.Response> GetOutgoingAsync(
        GetOutgoingInvoices.Request request,
        CancellationToken cancellationToken);
}