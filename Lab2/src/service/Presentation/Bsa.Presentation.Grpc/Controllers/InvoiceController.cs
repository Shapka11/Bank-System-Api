using Bsa.Application.Contracts.Invoices;
using Bsa.Application.Contracts.Invoices.Operations;
using Bsa.CsharpBackend.Grpc;
using Bsa.Presentation.Grpc.Mapping.Invoices;
using Bsa.Presentation.Grpc.Mapping.Invoices.Requests;
using Grpc.Core;
using System.Text.Json;

namespace Bsa.Presentation.Grpc.Controllers;

public sealed class InvoiceController : InvoiceService.InvoiceServiceBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public override async Task<ProtoCreateInvoiceResponse> CreateInvoice(
        ProtoCreateInvoiceRequest request,
        ServerCallContext context)
    {
        CreateInvoice.Response applicationResponse = await _invoiceService.CreateAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            CreateInvoice.Response.Success success => new ProtoCreateInvoiceResponse(success.Invoice.MapToProto()),
            CreateInvoice.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                unauthorized.ErrorMessage)),
            CreateInvoice.Response.SenderAccountNotFound senderAccountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Sender {senderAccountNotFound.AccountNumber} not found")),
            CreateInvoice.Response.ReceiverAccountNotFound receiverAccountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Receiver {receiverAccountNotFound.AccountNumber} not found")),
            CreateInvoice.Response.Forbidden forbidden => throw new RpcException(new Status(
                StatusCode.PermissionDenied,
                forbidden.Message)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }

    public override async Task<ProtoPayInvoiceResponse> PayInvoice(
        ProtoPayInvoiceRequest request,
        ServerCallContext context)
    {
        PayInvoice.Response applicationResponse = await _invoiceService.PayAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            PayInvoice.Response.Success success => new ProtoPayInvoiceResponse(success.Invoice.MapToProto()),
            PayInvoice.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                unauthorized.ErrorMessage)),
            PayInvoice.Response.InvoiceNotFound invoiceNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Invoice {invoiceNotFound.InvoiceId} not found")),
            PayInvoice.Response.InvalidInvoiceState invoiceState => throw new RpcException(
                new Status(StatusCode.FailedPrecondition, $"Invalid state: {invoiceState.State}")),
            PayInvoice.Response.AccountNotFound n => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Account {n.AccountNumber} not found")),
            PayInvoice.Response.Forbidden forbidden => throw new RpcException(
                new Status(StatusCode.PermissionDenied, forbidden.Message)),
            PayInvoice.Response.WithdrawalError withdrawalError => throw new RpcException(new Status(
                StatusCode.FailedPrecondition,
                withdrawalError.Message)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }

    public override async Task<ProtoRevokeInvoiceResponse> RevokeInvoice(
        ProtoRevokeInvoiceRequest request,
        ServerCallContext context)
    {
        RevokeInvoice.Response applicationResponse = await _invoiceService.RevokeAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            RevokeInvoice.Response.Success success => new ProtoRevokeInvoiceResponse(success.Invoice.MapToProto()),
            RevokeInvoice.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                unauthorized.ErrorMessage)),
            RevokeInvoice.Response.InvoiceNotFound invoiceNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Invoice {invoiceNotFound.InvoiceId} not found")),
            RevokeInvoice.Response.InvalidInvoiceState invoiceState => throw new RpcException(
                new Status(StatusCode.FailedPrecondition, invoiceState.Message)),
            RevokeInvoice.Response.AccountNotFound accountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Account {accountNotFound.AccountNumber} not found")),
            RevokeInvoice.Response.Forbidden forbidden => throw new RpcException(new Status(
                StatusCode.PermissionDenied,
                forbidden.Message)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }

    public override async Task<ProtoGetOutgoingInvoicesResponse> GetOutgoingInvoice(
        ProtoGetOutgoingInvoicesRequest request,
        ServerCallContext context)
    {
        GetOutgoingInvoices.Response applicationResponse = await _invoiceService.GetOutgoingAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            GetOutgoingInvoices.Response.Success success => new ProtoGetOutgoingInvoicesResponse(
                success.Invoices.Select(i => i.MapToProto()),
                success.PageToken is not null ? JsonSerializer.Serialize(success.PageToken.Value) : null),
            GetOutgoingInvoices.Response.InvalidStatus invalidStatus => throw new RpcException(
                new Status(StatusCode.InvalidArgument, invalidStatus.Message)),
            GetOutgoingInvoices.Response.AccountNotFound accountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                accountNotFound.Message)),
            GetOutgoingInvoices.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                unauthorized.ErrorMessage)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }

    public override async Task<ProtoGetIncomingInvoicesResponse> GetIncomingInvoice(
        ProtoGetIncomingInvoicesRequest request,
        ServerCallContext context)
    {
        GetIncomingInvoices.Response applicationResponse = await _invoiceService.GetIncomingAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            GetIncomingInvoices.Response.Success success => new ProtoGetIncomingInvoicesResponse(
                success.Invoices.Select(i => i.MapToProto()),
                success.PageToken is not null ? JsonSerializer.Serialize(success.PageToken.Value) : null),
            GetIncomingInvoices.Response.InvalidStatus invalidStatus => throw new RpcException(
                new Status(StatusCode.InvalidArgument, invalidStatus.Message)),
            GetIncomingInvoices.Response.AccountNotFound accountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                accountNotFound.Message)),
            GetIncomingInvoices.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                unauthorized.ErrorMessage)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }
}