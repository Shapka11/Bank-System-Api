using Bsa.Gateway.Application.Abstractions.Invoices;
using Bsa.Gateway.Application.Abstractions.Invoices.Operations;
using Bsa.Gateway.Application.Contracts.Invoices;
using Bsa.Gateway.Application.Contracts.Invoices.Operations.Requests;
using Bsa.Gateway.Application.Contracts.Invoices.Operations.Responses;
using Bsa.Gateway.Application.Mapping;
using System.Diagnostics;

namespace Bsa.Gateway.Application.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IInvoiceClient _invoiceClient;

    public InvoiceService(IInvoiceClient invoiceClient)
    {
        _invoiceClient = invoiceClient;
    }

    public async Task<CreateInvoiceResponse> CreateAsync(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new CreateInvoice.Request(
            request.SessionId,
            request.SenderAccountNumber,
            request.ReceiverAccountNumber,
            request.Amount);
        CreateInvoice.Response clientResponse = await _invoiceClient.CreateAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            CreateInvoice.Response.Failure failure => new CreateInvoiceResponse.Failure(failure.ErrorMessage),
            CreateInvoice.Response.Success success => new CreateInvoiceResponse.Success(success.BankInvoice.MapToDto()),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<PayInvoiceResponse> PayAsync(PayInvoiceRequest request, CancellationToken cancellationToken)
    {
        var clientRequest = new PayInvoice.Request(request.SessionId, request.InvoiceId);
        PayInvoice.Response clientResponse = await _invoiceClient.PayAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            PayInvoice.Response.Failure failure => new PayInvoiceResponse.Failure(failure.ErrorMessage),
            PayInvoice.Response.Success success => new PayInvoiceResponse.Success(success.BankInvoice.MapToDto()),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<RevokeInvoiceResponse> RevokeAsync(
        RevokeInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new RevokeInvoice.Request(request.SessionId, request.InvoiceId);
        RevokeInvoice.Response clientResponse = await _invoiceClient.RevokeAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            RevokeInvoice.Response.Failure failure => new RevokeInvoiceResponse.Failure(failure.ErrorMessage),
            RevokeInvoice.Response.Success success => new RevokeInvoiceResponse.Success(success.BankInvoice.MapToDto()),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetOutgoingInvoicesResponse> GetOutgoingAsync(
        GetOutgoingInvoicesRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new GetOutgoingInvoices.Request(
            request.SessionId,
            request.ReceiverAccountNumbers,
            request.Statuses.MapToBankModel(),
            request.PageSize,
            request.PageToken);
        GetOutgoingInvoices.Response clientResponse = await _invoiceClient
            .GetOutgoingAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            GetOutgoingInvoices.Response.Failure failure
                => new GetOutgoingInvoicesResponse.Failure(failure.ErrorMessage),
            GetOutgoingInvoices.Response.Success success
                => new GetOutgoingInvoicesResponse.Success(success.Invoices.MapToDto(), success.PageToken),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetIncomingInvoicesResponse> GetIncomingAsync(
        GetIncomingInvoicesRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new GetIncomingInvoices.Request(
            request.SessionId,
            request.SenderAccountNumbers,
            request.Statuses.MapToBankModel(),
            request.PageSize,
            request.PageToken);
        GetIncomingInvoices.Response clientResponse = await _invoiceClient
            .GetIncomingAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            GetIncomingInvoices.Response.Failure failure
                => new GetIncomingInvoicesResponse.Failure(failure.ErrorMessage),
            GetIncomingInvoices.Response.Success success
                => new GetIncomingInvoicesResponse.Success(success.Invoices.MapToDto(), success.PageToken),
            _ => throw new UnreachableException(),
        };
    }
}