using Bsa.Gateway.Application.Contracts.HistoryOperations;
using Bsa.Gateway.Application.Contracts.HistoryOperations.Operations;
using Bsa.Gateway.Presentation.Http.Mapping.HistoryOperations;
using Bsa.Gateway.Presentation.Http.Mapping.HistoryOperations.Requests;
using Bsa.Gateway.Presentation.Http.Requests.HistoryOperations;
using Bsa.Gateway.Presentation.Http.Responses.HistoryOperations;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mime;

namespace Bsa.Gateway.Presentation.Http.Controllers;

[ApiController]
[Route("api/history-operations")]
public sealed class HistoryOperationController : ControllerBase
{
    private readonly IHistoryOperationService _historyOperationService;

    public HistoryOperationController(IHistoryOperationService historyOperationService)
    {
        _historyOperationService = historyOperationService;
    }

    [HttpGet]
    [EndpointName("GetHistory")]
    [EndpointSummary("Get operation history")]
    [EndpointDescription("Returns a paginated list of all account operations (deposits, withdrawals, transfers).")]
    [ProducesResponseType<GetHistoryHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<GetHistoryHttpResponse>> GetHistory(
        [FromQuery] GetHistoryHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        GetHistoryOperationsResponse response = await _historyOperationService.GetAsync(
            httpRequest.MapToApplication(),
            cancellationToken);

        return response switch
        {
            GetHistoryOperationsResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            GetHistoryOperationsResponse.Success success => Ok(new GetHistoryHttpResponse
            {
                History = success.History.MapToModel(),
                PageToken = success.PageToken,
            }),
            _ => throw new UnreachableException(),
        };
    }
}