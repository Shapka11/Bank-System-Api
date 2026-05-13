using BankSystemApi.Gateway.Application.Contracts.Invoices;
using BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Responses;
using BankSystemApi.Gateway.Presentation.Http.Extensions;
using BankSystemApi.Gateway.Presentation.Http.Features;
using BankSystemApi.Gateway.Presentation.Http.Mapping.Invoices;
using BankSystemApi.Gateway.Presentation.Http.Mapping.Invoices.Requests;
using BankSystemApi.Gateway.Presentation.Http.Requests.Invoices;
using BankSystemApi.Gateway.Presentation.Http.Responses.Invoices;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mime;

namespace BankSystemApi.Gateway.Presentation.Http.Controllers;

[ApiController]
[Route("api/invoices")]
[Produces(MediaTypeNames.Application.Json)]
public sealed class InvoiceController : ControllerBase
{
    private const string Scope = "Invoices";
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpPost]
    [AuthorizeFeature(Scope)]
    [EndpointName("CreateInvoice")]
    [EndpointSummary("Create an invoice")]
    [EndpointDescription("Creates a new payment request (invoice) between two accounts.")]
    [ProducesResponseType<CreateInvoiceHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<CreateInvoiceHttpResponse>> Create(
        [FromBody] CreateInvoiceHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        string userId = User.GetUserId();

        CreateInvoiceResponse response = await _invoiceService.CreateAsync(
            httpRequest.MapToApplication(userId),
            cancellationToken);

        return response switch
        {
            CreateInvoiceResponse.Success success => Ok(new CreateInvoiceHttpResponse
            {
                Invoice = success.Invoice.MapToModel(),
            }),
            CreateInvoiceResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("pay")]
    [AuthorizeFeature(Scope)]
    [EndpointName("PayInvoice")]
    [EndpointSummary("Pay an invoice")]
    [EndpointDescription("Executes a payment for the specified invoice. Sets the invoice status to 'Paid'.")]
    [ProducesResponseType<PayInvoiceHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<PayInvoiceHttpResponse>> Pay(
        [FromBody] PayInvoiceHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        string userId = User.GetUserId();

        PayInvoiceResponse response = await _invoiceService.PayAsync(
            httpRequest.MapToApplication(userId),
            cancellationToken);

        return response switch
        {
            PayInvoiceResponse.Success success => Ok(new PayInvoiceHttpResponse
            {
                Invoice = success.Invoice.MapToModel(),
            }),
            PayInvoiceResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("revoke")]
    [AuthorizeFeature(Scope)]
    [EndpointName("RevokeInvoice")]
    [EndpointSummary("Revoke an invoice")]
    [EndpointDescription("Cancels an invoice. Only allowed for invoices with 'Pending' status.")]
    [ProducesResponseType<RevokeInvoiceHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<RevokeInvoiceHttpResponse>> Revoke(
        [FromBody] RevokeInvoiceHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        string userId = User.GetUserId();

        RevokeInvoiceResponse response = await _invoiceService.RevokeAsync(
            httpRequest.MapToApplication(userId),
            cancellationToken);

        return response switch
        {
            RevokeInvoiceResponse.Success success => Ok(new RevokeInvoiceHttpResponse
            {
                Invoice = success.Invoice.MapToModel(),
            }),
            RevokeInvoiceResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }

    [HttpGet("outgoing")]
    [AuthorizeFeature(Scope)]
    [EndpointName("GetOutgoingInvoices")]
    [EndpointSummary("Get outgoing invoices")]
    [EndpointDescription("Returns a list of invoices issued by the specified account (sender).")]
    [ProducesResponseType<GetOutgoingInvoicesHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<GetOutgoingInvoicesHttpResponse>> GetOutgoing(
        [FromQuery] GetOutgoingInvoicesHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        string userId = User.GetUserId();

        GetOutgoingInvoicesResponse response = await _invoiceService.GetOutgoingAsync(
            httpRequest.MapToApplication(userId),
            cancellationToken);

        return response switch
        {
            GetOutgoingInvoicesResponse.Success success => Ok(new GetOutgoingInvoicesHttpResponse
            {
                Invoices = success.Invoices.Select(i => i.MapToModel()).ToArray(),
                PageToken = success.PageToken,
            }),
            GetOutgoingInvoicesResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }

    [HttpGet("incoming")]
    [AuthorizeFeature(Scope)]
    [EndpointName("GetIncomingInvoices")]
    [EndpointSummary("Get incoming invoices")]
    [EndpointDescription("Returns a list of invoices issued to the specified account (receiver).")]
    [ProducesResponseType<GetIncomingInvoicesHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<GetIncomingInvoicesHttpResponse>> GetIncoming(
        [FromQuery] GetIncomingInvoicesHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        string userId = User.GetUserId();

        GetIncomingInvoicesResponse response = await _invoiceService.GetIncomingAsync(
            httpRequest.MapToApplication(userId),
            cancellationToken);

        return response switch
        {
            GetIncomingInvoicesResponse.Success success => Ok(new GetIncomingInvoicesHttpResponse
            {
                Invoices = success.Invoices.Select(i => i.MapToModel()).ToArray(),
                PageToken = success.PageToken,
            }),
            GetIncomingInvoicesResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }
}