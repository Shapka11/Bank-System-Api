using Bsa.CsharpBackend.Grpc;
using Bsa.Gateway.Application.Abstractions.Invoices;
using Bsa.Gateway.Application.Abstractions.Invoices.Operations;
using Bsa.Gateway.Infrastructure.BankService.Mapping;
using Google.Type;

namespace Bsa.Gateway.Infrastructure.BankService.Clients;

public sealed class InvoiceClient : IInvoiceClient
{
    private readonly InvoiceService.InvoiceServiceClient _invoiceClient;

    public InvoiceClient(InvoiceService.InvoiceServiceClient invoiceClient)
    {
        _invoiceClient = invoiceClient;
    }

    public async Task<CreateInvoice.Response> CreateAsync(
        CreateInvoice.Request request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoCreateInvoiceRequest(
            request.SessionId.ToString(),
            request.SenderAccountNumber,
            request.ReceiverAccountNumber,
            new Money { DecimalValue = request.Amount });

        ProtoCreateInvoiceResponse clientResponse = await _invoiceClient.CreateInvoiceAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new CreateInvoice.Response.Success(clientResponse.Invoice.MapToModel());
    }

    public async Task<PayInvoice.Response> PayAsync(PayInvoice.Request request, CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoPayInvoiceRequest(request.SessionId.ToString(), request.InvoiceId);

        ProtoPayInvoiceResponse clientResponse = await _invoiceClient.PayInvoiceAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new PayInvoice.Response.Success(clientResponse.Invoice.MapToModel());
    }

    public async Task<RevokeInvoice.Response> RevokeAsync(
        RevokeInvoice.Request request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoRevokeInvoiceRequest(
            request.SessionId.ToString(),
            request.InvoiceId);

        ProtoRevokeInvoiceResponse clientResponse = await _invoiceClient.RevokeInvoiceAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new RevokeInvoice.Response.Success(clientResponse.Invoice.MapToModel());
    }

    public async Task<GetOutgoingInvoices.Response> GetOutgoingAsync(
        GetOutgoingInvoices.Request request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoGetOutgoingInvoicesRequest(
            request.SessionId.ToString(),
            request.ReceiverAccountNumbers.MapToProto(),
            request.Statuses.MapToProto(),
            new ProtoPagination(request.PageSize, request.PageToken));

        ProtoGetOutgoingInvoicesResponse clientResponse = await _invoiceClient.GetOutgoingInvoiceAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new GetOutgoingInvoices.Response.Success(
            clientResponse.Invoices.MapToModel(),
            clientResponse.PageToken);
    }

    public async Task<GetIncomingInvoices.Response> GetIncomingAsync(
        GetIncomingInvoices.Request request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoGetIncomingInvoicesRequest(
            request.SessionId.ToString(),
            request.SenderAccountNumbers.MapToProto(),
            request.Statuses.MapToProto(),
            new ProtoPagination(request.PageSize, request.PageToken));

        ProtoGetIncomingInvoicesResponse clientResponse = await _invoiceClient.GetIncomingInvoiceAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new GetIncomingInvoices.Response.Success(
            clientResponse.Invoices.MapToModel(),
            clientResponse.PageToken);
    }
}