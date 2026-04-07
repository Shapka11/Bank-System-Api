using System.ComponentModel.DataAnnotations;

namespace Bsa.Application.Options;

public sealed class SecurityOptions
{
    [MinLength(4,  ErrorMessage = "Password must have at least 4 characters")]
    public required string SystemPassword { get; init; }
}