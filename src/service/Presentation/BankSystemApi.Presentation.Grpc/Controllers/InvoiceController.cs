using BankSystemApi.Application.Contracts.Invoices;
using BankSystemApi.Application.Contracts.Invoices.Operations;
using BankSystemApi.Invoices.Grpc;
using BankSystemApi.Presentation.Grpc.Mapping.Invoices;
using BankSystemApi.Presentation.Grpc.Mapping.Invoices.Requests;
using Grpc.Core;
using System.Text.Json;

namespace BankSystemApi.Presentation.Grpc.Controllers;

public sealed class InvoiceController : InvoiceService.InvoiceServiceBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public override async Task<ProtoCreateInvoiceResponse> Create(
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
                $"User {unauthorized.UserId} is not authenticated")),
            CreateInvoice.Response.SenderAccountNotFound senderAccountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Sender {senderAccountNotFound.AccountId} not found")),
            CreateInvoice.Response.ReceiverAccountNotFound receiverAccountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Receiver {receiverAccountNotFound.AccountId} not found")),
            CreateInvoice.Response.Forbidden forbidden => throw new RpcException(new Status(
                StatusCode.PermissionDenied,
                forbidden.Message)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }

    public override async Task<ProtoPayInvoiceResponse> Pay(
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
                $"User {unauthorized.UserId} is not authenticated")),
            PayInvoice.Response.InvoiceNotFound invoiceNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Invoice {invoiceNotFound.InvoiceId} not found")),
            PayInvoice.Response.InvalidInvoiceState invoiceState => throw new RpcException(
                new Status(StatusCode.FailedPrecondition, $"Invalid state: {invoiceState.State}")),
            PayInvoice.Response.AccountNotFound accountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Account {accountNotFound.AccountId} not found")),
            PayInvoice.Response.Forbidden forbidden => throw new RpcException(
                new Status(StatusCode.PermissionDenied, forbidden.Message)),
            PayInvoice.Response.WithdrawalError withdrawalError => throw new RpcException(new Status(
                StatusCode.FailedPrecondition,
                $"Account with id {withdrawalError.AccountId} has: {withdrawalError.Message}")),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }

    public override async Task<ProtoRevokeInvoiceResponse> Revoke(
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
                $"User {unauthorized.UserId} is not authenticated")),
            RevokeInvoice.Response.InvoiceNotFound invoiceNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Invoice {invoiceNotFound.InvoiceId} not found")),
            RevokeInvoice.Response.InvalidInvoiceState invoiceState => throw new RpcException(
                new Status(StatusCode.FailedPrecondition, invoiceState.Message)),
            RevokeInvoice.Response.AccountNotFound accountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Account {accountNotFound.AccountId} not found")),
            RevokeInvoice.Response.Forbidden forbidden => throw new RpcException(new Status(
                StatusCode.PermissionDenied,
                forbidden.Message)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }

    public override async Task<ProtoGetInvoicesResponse> Get(
        ProtoGetInvoicesRequest request,
        ServerCallContext context)
    {
        GetInvoices.Response applicationResponse = await _invoiceService.GetAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            GetInvoices.Response.Success success => new ProtoGetInvoicesResponse(
                success.Invoices.Select(i => i.MapToProto()),
                success.PageToken is not null ? JsonSerializer.Serialize(success.PageToken.Value) : null),
            GetInvoices.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                $"User {unauthorized.UserId} is not authenticated")),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }
}