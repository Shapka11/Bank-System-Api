using Bsa.Application.Contracts.Users;
using Bsa.Application.Contracts.Users.Operations;
using Bsa.Presentation.Http.Models.User;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace Bsa.Presentation.Http.Controllers;

[ApiController]
[Route("api/user")]
[Produces("application/json")]
public sealed class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginUserRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new LoginUser.Request(httpRequest.AccountNumber, httpRequest.Password);
        LoginUser.Response response = await _userService.LoginAsync(request, cancellationToken);

        return response switch
        {
            LoginUser.Response.Success success => Ok(success.UserSession),
            LoginUser.Response.Failure failure => BadRequest(failure.ErrorMessage),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(
        [FromBody] LogoutUserRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new LogoutUser.Request(httpRequest.Id);
        LogoutUser.Response response = await _userService.LogoutAsync(request, cancellationToken);

        return response switch
        {
            LogoutUser.Response.Success => Ok(),
            LogoutUser.Response.Failure failure => BadRequest(failure.ErrorMessage),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> DepositAsync(
        [FromBody] DepositRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new DepositUserAccount.Request(httpRequest.Id, httpRequest.Amount);
        DepositUserAccount.Response response = await _userService.DepositAsync(request, cancellationToken);

        return response switch
        {
            DepositUserAccount.Response.Success success => Ok(success.Account),
            DepositUserAccount.Response.Failure failure => BadRequest(failure.ErrorMessage),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> WithdrawAsync(
        [FromBody] WithdrawRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new WithdrawUserAccount.Request(httpRequest.Id, httpRequest.Amount);
        WithdrawUserAccount.Response response = await _userService.WithdrawAsync(request, cancellationToken);

        return response switch
        {
            WithdrawUserAccount.Response.Success success => Ok(success.Account),
            WithdrawUserAccount.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalanceAsync(
        [FromQuery] GetBalanceRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new GetUserBalance.Request(httpRequest.Id);
        GetUserBalance.Response response = await _userService.GetBalanceAsync(request, cancellationToken);

        return response switch
        {
            GetUserBalance.Response.Success success => Ok(success),
            GetUserBalance.Response.Failure failure => BadRequest(failure.ErrorMessage),
            _ => throw new UnreachableException(),
        };
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistoryAsync(
        [FromQuery] GetHistoryRequest httpRequest,
        CancellationToken cancellationToken)
    {
        GetUserOperationHistory.PageToken? pageToken = httpRequest.PageToken is null
            ? null
            : JsonSerializer.Deserialize<GetUserOperationHistory.PageToken>(httpRequest.PageToken);

        var request = new GetUserOperationHistory.Request(httpRequest.Id, httpRequest.PageSize, pageToken);
        GetUserOperationHistory.Response response = await _userService.GetHistoryAsync(request, cancellationToken);

        if (response is GetUserOperationHistory.Response.Failure failure)
        {
            return BadRequest(failure.Message);
        }

        if (response is GetUserOperationHistory.Response.Success success)
        {
            string? responsePageToken = success.PageToken is null
                ? null
                : JsonSerializer.Serialize(success.PageToken.Value);

            var httpRespone = new GetHistoryResponse
            {
                PageToken = responsePageToken,
                History = success.History,
            };

            return Ok(httpRespone);
        }

        throw new UnreachableException();
    }
}