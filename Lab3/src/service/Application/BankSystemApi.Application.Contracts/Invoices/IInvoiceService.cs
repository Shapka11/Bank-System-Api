using BankSystemApi.Application.Contracts.Invoices.Operations;

namespace BankSystemApi.Application.Contracts.Invoices;

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

    Task<GetInvoices.Response> GetAsync(
        GetInvoices.Request request,
        CancellationToken cancellationToken);
}