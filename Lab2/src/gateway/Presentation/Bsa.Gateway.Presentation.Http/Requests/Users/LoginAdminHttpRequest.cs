using System.ComponentModel.DataAnnotations;

namespace Bsa.Gateway.Presentation.Http.Requests.Users;

public readonly record struct LoginAdminHttpRequest
{
    [MinLength(4,  ErrorMessage = "Password must have at least 4 characters")]
    public required string Password { get; init; }
}