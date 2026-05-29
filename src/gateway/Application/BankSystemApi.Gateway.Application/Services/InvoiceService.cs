using BankSystemApi.Gateway.Application.Abstractions.Invoices;
using BankSystemApi.Gateway.Application.Abstractions.Invoices.Operations;
using BankSystemApi.Gateway.Application.Abstractions.Users;
using BankSystemApi.Gateway.Application.Abstractions.Users.Models;
using BankSystemApi.Gateway.Application.Abstractions.Users.Operations;
using BankSystemApi.Gateway.Application.Contracts.Invoices;
using BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Requests;
using BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Responses;
using BankSystemApi.Gateway.Application.Mapping.Invoices;
using System.Diagnostics;

namespace BankSystemApi.Gateway.Application.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IInvoiceClient _invoiceClient;
    private readonly IUserClient _userClient;
    private readonly IInvoiceApprovalClient _invoiceApprovalClient;

    public InvoiceService(
        IInvoiceClient invoiceClient,
        IInvoiceApprovalClient invoiceApprovalClient,
        IUserClient userClient)
    {
        _invoiceClient = invoiceClient;
        _invoiceApprovalClient = invoiceApprovalClient;
        _userClient = userClient;
    }

    public async Task<CreateInvoiceResponse> CreateAsync(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new CreateInvoice.Request(
            request.UserId,
            request.SenderAccountId,
            request.ReceiverAccountId,
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
        var clientRequest = new PayInvoice.Request(request.UserId, request.InvoiceId);
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
        var clientRequest = new RevokeInvoice.Request(request.UserId, request.InvoiceId);
        RevokeInvoice.Response clientResponse = await _invoiceClient.RevokeAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            RevokeInvoice.Response.Failure failure => new RevokeInvoiceResponse.Failure(failure.ErrorMessage),
            RevokeInvoice.Response.Success success => new RevokeInvoiceResponse.Success(success.BankInvoice.MapToDto()),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<ApproveInvoiceResponse> ApproveAsync(
        ApproveInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var getUsersRequest = new GetUsers.Request([request.UserId], [], 1);
        GetUsers.Response users = await _userClient.GetAsync(getUsersRequest, cancellationToken);

        if (users is not GetUsers.Response.Success successUserRequest)
        {
            if (users is GetUsers.Response.Failure failureUserRequest)
            {
                return new ApproveInvoiceResponse.Failure(failureUserRequest.ErrorMessage);
            }

            throw new UnreachableException();
        }

        BankUserModel user = successUserRequest.Users.First();

        var clientRequest = new ApproveInvoice.Request(user.Id, request.InvoiceId);
        ApproveInvoice.Response clientResponse = await _invoiceApprovalClient
            .ApproveAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            ApproveInvoice.Response.Failure failure => new ApproveInvoiceResponse.Failure(failure.ErrorMessage),
            ApproveInvoice.Response.Success => new ApproveInvoiceResponse.Success(),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<DeclineInvoiceResponse> DeclineAsync(
        DeclineInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var getUsersRequest = new GetUsers.Request([request.UserId], [], 1);
        GetUsers.Response users = await _userClient.GetAsync(getUsersRequest, cancellationToken);

        if (users is not GetUsers.Response.Success successUserRequest)
        {
            if (users is GetUsers.Response.Failure failureUserRequest)
            {
                return new DeclineInvoiceResponse.Failure(failureUserRequest.ErrorMessage);
            }

            throw new UnreachableException();
        }

        BankUserModel user = successUserRequest.Users.First();

        var clientRequest = new DeclineInvoice.Request(user.Id, request.InvoiceId);
        DeclineInvoice.Response clientResponse = await _invoiceApprovalClient
            .DeclineAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            DeclineInvoice.Response.Failure failure => new DeclineInvoiceResponse.Failure(failure.ErrorMessage),
            DeclineInvoice.Response.Success => new DeclineInvoiceResponse.Success(),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<AssignAccountantResponse> AssignAccountantAsync(
        AssignAccountantRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new AssignAccountant.Request(request.InvoiceId, request.AccountantId);
        AssignAccountant.Response clientResponse = await _invoiceApprovalClient
            .AssignAccountantAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            AssignAccountant.Response.Failure failure => new AssignAccountantResponse.Failure(failure.ErrorMessage),
            AssignAccountant.Response.Success => new AssignAccountantResponse.Success(),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetOutgoingInvoicesResponse> GetOutgoingAsync(
        GetOutgoingInvoicesRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new GetOutgoingInvoices.Request(
            request.UserId,
            request.ReceiverAccountIds,
            request.Statuses.Select(s => s.MapToBankModel()),
            request.PageSize,
            request.PageToken);
        GetOutgoingInvoices.Response clientResponse = await _invoiceClient
            .GetOutgoingAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            GetOutgoingInvoices.Response.Failure failure
                => new GetOutgoingInvoicesResponse.Failure(failure.ErrorMessage),
            GetOutgoingInvoices.Response.Success success
                => new GetOutgoingInvoicesResponse.Success(
                    success.Invoices.Select(i => i.MapToDto()).ToArray(),
                    success.PageToken),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetIncomingInvoicesResponse> GetIncomingAsync(
        GetIncomingInvoicesRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new GetIncomingInvoices.Request(
            request.UserId,
            request.SenderAccountIds,
            request.Statuses.Select(s => s.MapToBankModel()),
            request.PageSize,
            request.PageToken);
        GetIncomingInvoices.Response clientResponse = await _invoiceClient
            .GetIncomingAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            GetIncomingInvoices.Response.Failure failure
                => new GetIncomingInvoicesResponse.Failure(failure.ErrorMessage),
            GetIncomingInvoices.Response.Success success
                => new GetIncomingInvoicesResponse.Success(
                    success.Invoices.Select(i => i.MapToDto()).ToArray(),
                    success.PageToken),
            _ => throw new UnreachableException(),
        };
    }
}