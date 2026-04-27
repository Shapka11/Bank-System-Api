using BankSystemApi.Gateway.Application.Abstractions.Invoices;
using BankSystemApi.Gateway.Application.Abstractions.Invoices.Operations;
using BankSystemApi.Gateway.Infrastructure.BankService.Mapping;
using BankSystemApi.Grpc;
using Google.Type;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Clients;

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
            request.UserId.ToString(),
            request.SenderAccountId.ToString(),
            request.ReceiverAccountId.ToString(),
            new Money { DecimalValue = request.Amount });

        ProtoCreateInvoiceResponse clientResponse = await _invoiceClient.CreateInvoiceAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new CreateInvoice.Response.Success(clientResponse.Invoice.MapToModel());
    }

    public async Task<PayInvoice.Response> PayAsync(PayInvoice.Request request, CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoPayInvoiceRequest(request.UserId.ToString(), request.InvoiceId);

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
            request.UserId.ToString(),
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
        var clientRequest = new ProtoGetInvoicesRequest(
            request.UserId.ToString(),
            request.ReceiverAccountIds.Select(rai => rai.ToString()),
            request.Statuses.Select(s => s.MapToProto()),
            ProtoInvoiceType.Outgoing,
            new ProtoPagination(request.PageSize, request.PageToken));

        ProtoGetInvoicesResponse clientResponse = await _invoiceClient.GetInvoiceAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new GetOutgoingInvoices.Response.Success(
            clientResponse.Invoices.Select(i => i.MapToModel()).ToArray(),
            clientResponse.PageToken);
    }

    public async Task<GetIncomingInvoices.Response> GetIncomingAsync(
        GetIncomingInvoices.Request request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoGetInvoicesRequest(
            request.UserId.ToString(),
            request.SenderAccountIds.Select(sai => sai.ToString()),
            request.Statuses.Select(s => s.MapToProto()),
            ProtoInvoiceType.Incoming,
            new ProtoPagination(request.PageSize, request.PageToken));

        ProtoGetInvoicesResponse clientResponse = await _invoiceClient.GetInvoiceAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new GetIncomingInvoices.Response.Success(
            clientResponse.Invoices.Select(i => i.MapToModel()).ToArray(),
            clientResponse.PageToken);
    }
}