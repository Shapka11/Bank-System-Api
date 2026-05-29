using BankSystemApi.ApprovalService.Invoices.Grpc;
using BankSystemApi.Gateway.Application.Abstractions.Invoices;
using BankSystemApi.Gateway.Application.Abstractions.Invoices.Operations;
using BankSystemApi.Gateway.Infrastructure.ApprovalService.Activities;
using BankSystemApi.Gateway.Infrastructure.ApprovalService.Extensions;
using Grpc.Net.ClientFactory;
using System.Diagnostics;

namespace BankSystemApi.Gateway.Infrastructure.ApprovalService.Clients;

public sealed class InvoiceApprovalClient : IInvoiceApprovalClient
{
    private readonly InvoiceService.InvoiceServiceClient _invoiceApprovalClient;

    public InvoiceApprovalClient(GrpcClientFactory grpcClientFactory)
    {
        _invoiceApprovalClient = grpcClientFactory
            .CreateClient<InvoiceService.InvoiceServiceClient>("service-invoice-approval");
    }

    public async Task<ApproveInvoice.Response> ApproveAsync(
        ApproveInvoice.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = InvoiceApprovalClientActivity.ActivitySource.StartActivity();
        activity.AddUserIdBaggage(request.UserId);
        activity.AddInvoiceIdBaggage(request.InvoiceId);

        var clientRequest = new ProtoApproveInvoiceRequest(request.InvoiceId, request.UserId);

        await _invoiceApprovalClient.ApproveInvoiceAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new ApproveInvoice.Response.Success();
    }

    public async Task<DeclineInvoice.Response> DeclineAsync(
        DeclineInvoice.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = InvoiceApprovalClientActivity.ActivitySource.StartActivity();
        activity.AddUserIdBaggage(request.UserId);
        activity.AddInvoiceIdBaggage(request.InvoiceId);

        var clientRequest = new ProtoDeclineInvoiceRequest(request.InvoiceId, request.UserId);

        await _invoiceApprovalClient.DeclineInvoiceAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new DeclineInvoice.Response.Success();
    }

    public async Task<AssignAccountant.Response> AssignAccountantAsync(
        AssignAccountant.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = InvoiceApprovalClientActivity.ActivitySource.StartActivity();
        activity.AddUserIdBaggage(request.UserId);
        activity.AddInvoiceIdBaggage(request.InvoiceId);

        var clientRequest = new ProtoAssignAccountantRequest(request.InvoiceId, request.UserId);

        await _invoiceApprovalClient.AssignAccountantAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new AssignAccountant.Response.Success();
    }
}