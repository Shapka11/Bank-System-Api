using Bsa.Gateway.Presentation.Http.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Bsa.Gateway.Presentation.Http.Requests.Users;

public sealed class LoginAdminHttpRequest
{
    [MinLength(4,  ErrorMessage = "Password must have at least 4 characters")]
    [NotWhiteSpace]
    public required string Password { get; init; }
}