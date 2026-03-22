using Bsa.Application.Contracts.Admins;
using Bsa.Application.Contracts.Admins.Operations;
using Bsa.Presentation.Http.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bsa.Presentation.Http.Controllers;

[ApiController]
[Route("api/admin")]
public sealed class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginAdminRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new LoginAdmin.Request(httpRequest.Password);
        LoginAdmin.Response response = await _adminService.LoginAsync(request, cancellationToken);

        return response switch
        {
            LoginAdmin.Response.Success success => Ok(success.AdminSession),
            LoginAdmin.Response.Failure failure => BadRequest(failure.ErrorMessage),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(
        [FromBody] LogoutAdminRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new LogoutAdmin.Request(httpRequest.Id);
        LogoutAdmin.Response response = await _adminService.LogoutAsync(request, cancellationToken);

        return response switch
        {
            LogoutAdmin.Response.Success => Ok(),
            LogoutAdmin.Response.Failure failure => BadRequest(failure.ErrorMessage),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("account")]
    public async Task<IActionResult> CreateUserAccountAsync(
        [FromBody] CreateUserAccountRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var request = new CreateAccount.Request(httpRequest.Id, httpRequest.AccountNumber, httpRequest.Password);
        CreateAccount.Response response = await _adminService.CreateAccountAsync(request, cancellationToken);

        return response switch
        {
            CreateAccount.Response.Success success => Ok(success.Account),
            CreateAccount.Response.Failure failure => BadRequest(failure.ErrorMessage),
            _ => throw new UnreachableException(),
        };
    }
}