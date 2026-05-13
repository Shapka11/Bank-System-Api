using System.Security.Claims;

namespace BankSystemApi.Gateway.Presentation.Http.Extensions;

public static class ClaimsPrinsipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        string? id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (id is null)
            throw new ArgumentNullException(nameof(principal), "user id is null");

        return id;
    }
}