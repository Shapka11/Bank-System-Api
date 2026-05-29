using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace BankSystemApi.Gateway.Presentation.Http.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthenticationController : ControllerBase
{
    [HttpGet]
    public ActionResult Login(string? returnUri)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = returnUri ?? "/",
        };

        return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
    }
}