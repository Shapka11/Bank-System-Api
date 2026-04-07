using Bsa.Gateway.Application.Contracts.Users;
using Bsa.Gateway.Application.Contracts.Users.Operations;
using Bsa.Gateway.Presentation.Http.Mapping.Users;
using Bsa.Gateway.Presentation.Http.Mapping.Users.User;
using Bsa.Gateway.Presentation.Http.Requests.Users;
using Bsa.Gateway.Presentation.Http.Responses.Users;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mime;

namespace Bsa.Gateway.Presentation.Http.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    [EndpointName("LoginUser")]
    [EndpointSummary("User login")]
    [EndpointDescription("Authorizes a user via account number and password. Returns session data on success.")]
    [ProducesResponseType<LoginUserHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<LoginUserHttpResponse>> LoginUser(
        [FromBody] LoginUserHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        LoginUserResponse response = await _userService.LoginUserAsync(
            httpRequest.MapToApplication(),
            cancellationToken);

        return response switch
        {
            LoginUserResponse.Success success => Ok(new LoginUserHttpResponse
            {
                UserSession = success.UserSession.MapToModel(),
            }),
            LoginUserResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("logout")]
    [EndpointName("LogoutUser")]
    [EndpointSummary("User logout")]
    [EndpointDescription("Terminates a user session by session ID.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult> LogoutUser(
        [FromBody] LogoutUserHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        LogoutUserResponse response = await _userService.LogoutUserAsync(
            httpRequest.MapToApplication(),
            cancellationToken);

        return response switch
        {
            LogoutUserResponse.Success => Ok(),
            LogoutUserResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }
}