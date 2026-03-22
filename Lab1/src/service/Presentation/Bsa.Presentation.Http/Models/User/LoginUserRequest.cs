namespace Bsa.Presentation.Http.Models.User;

public sealed class LoginUserRequest
{
    public required string AccountNumber { get; init; }

    public required string Password { get; init; }
}