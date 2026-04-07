namespace Bsa.Gateway.Presentation.Http.Requests.Users;

public readonly record struct LoginUserHttpRequest
{
    public required string AccountNumber { get; init; }

    public required string Password { get; init; }
}