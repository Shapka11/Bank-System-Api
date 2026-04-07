using System.ComponentModel.DataAnnotations;

namespace Bsa.Gateway.Presentation.Http.Requests.Accounts;

public readonly record struct CreateAccountHttpRequest
{
    public required Guid SessionId { get; init; }

    public required string AccountNumber { get; init; }

    [MinLength(4,  ErrorMessage = "Password must have at least 4 characters")]
    public required string Password { get; init; }
}