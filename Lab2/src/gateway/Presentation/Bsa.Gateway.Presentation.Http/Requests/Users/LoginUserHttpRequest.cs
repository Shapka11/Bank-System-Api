using Bsa.Gateway.Presentation.Http.Attributes;

namespace Bsa.Gateway.Presentation.Http.Requests.Users;

public sealed class LoginUserHttpRequest
{
    [NotWhiteSpace]
    public required string AccountNumber { get; init; }

    [NotWhiteSpace]
    public required string Password { get; init; }
}