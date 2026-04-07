using Bsa.Gateway.Application.Contracts.Users;
using Bsa.Gateway.Application.Contracts.Users.Operations;
using Bsa.Gateway.Presentation.Http.Mapping.Users;
using Bsa.Gateway.Presentation.Http.Mapping.Users.Admin;
using Bsa.Gateway.Presentation.Http.Requests.Users;
using Bsa.Gateway.Presentation.Http.Responses.Users;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mime;

namespace Bsa.Gateway.Presentation.Http.Controllers;

[ApiController]
[Route("api/admins")]
public sealed class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("login")]
    [EndpointName("LoginAdmin")]
    [EndpointSummary("Admin login")]
    [EndpointDescription("Allows an administrator to log in using the master password. Returns session data.")]
    [ProducesResponseType<LoginAdminHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<LoginAdminHttpResponse>> LoginAdmin(
        [FromBody] LoginAdminHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        LoginAdminResponse response = await _adminService.LoginAdminAsync(
            httpRequest.MapToApplication(),
            cancellationToken);

        return response switch
        {
            LoginAdminResponse.Success success => Ok(new LoginAdminHttpResponse
            {
                AdminSession = success.AdminSession.MapToModel(),
            }),
            LoginAdminResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("logout")]
    [EndpointName("LogoutAdmin")]
    [EndpointSummary("Admin logout")]
    [EndpointDescription("Terminates an administrator session by session ID.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult> LogoutAdmin(
        [FromBody] LogoutAdminHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        LogoutAdminResponse response = await _adminService.LogoutAdminAsync(
            httpRequest.MapToApplication(),
            cancellationToken);

        return response switch
        {
            LogoutAdminResponse.Success => Ok(),
            LogoutAdminResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }
}