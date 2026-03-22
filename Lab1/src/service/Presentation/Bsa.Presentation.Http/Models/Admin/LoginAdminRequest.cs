using System.ComponentModel.DataAnnotations;

namespace Bsa.Presentation.Http.Models.Admin;

public sealed class LoginAdminRequest
{
    [MinLength(4,  ErrorMessage = "Password must have at least 4 characters")]
    [RegularExpression(@"^\S+$", ErrorMessage = "Password cannot contain spaces")]
    public required string Password { get; init; }
}