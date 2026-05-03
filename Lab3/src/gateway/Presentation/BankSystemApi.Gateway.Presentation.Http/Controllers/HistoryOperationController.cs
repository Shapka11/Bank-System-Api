using BankSystemApi.Gateway.Application.Contracts.HistoryOperations;
using BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Operations;
using BankSystemApi.Gateway.Presentation.Http.Extensions;
using BankSystemApi.Gateway.Presentation.Http.Features;
using BankSystemApi.Gateway.Presentation.Http.Mapping.HistoryOperations;
using BankSystemApi.Gateway.Presentation.Http.Mapping.HistoryOperations.Requests;
using BankSystemApi.Gateway.Presentation.Http.Requests.HistoryOperations;
using BankSystemApi.Gateway.Presentation.Http.Responses.HistoryOperations;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mime;

namespace BankSystemApi.Gateway.Presentation.Http.Controllers;

[ApiController]
[Route("api/history-operations")]
public sealed class HistoryOperationController : ControllerBase
{
    private const string Scope = "HistoryOperations";
    private readonly IHistoryOperationService _historyOperationService;

    public HistoryOperationController(IHistoryOperationService historyOperationService)
    {
        _historyOperationService = historyOperationService;
    }

    [HttpGet]
    [AuthorizeFeature(Scope)]
    [EndpointName("GetHistory")]
    [EndpointSummary("Get operation history")]
    [EndpointDescription("Returns a paginated list of all account operations (deposits, withdrawals, transfers).")]
    [ProducesResponseType<GetHistoryHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<GetHistoryHttpResponse>> Get(
        [FromQuery] GetHistoryHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        string userId = User.GetUserId();

        Activity.Current.AddUserIdBaggage(userId);
        Activity.Current.AddAccountIdBaggage(httpRequest.AccountId);

        GetHistoryOperationsResponse response = await _historyOperationService.GetAsync(
            httpRequest.MapToApplication(userId),
            cancellationToken);

        return response switch
        {
            GetHistoryOperationsResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            GetHistoryOperationsResponse.Success success => Ok(new GetHistoryHttpResponse
            {
                History = success.History.Select(h => h.MapToModel()).ToArray(),
                PageToken = success.PageToken,
            }),
            _ => throw new UnreachableException(),
        };
    }
}