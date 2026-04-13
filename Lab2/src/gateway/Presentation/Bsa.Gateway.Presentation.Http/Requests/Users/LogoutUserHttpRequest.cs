namespace Bsa.Gateway.Presentation.Http.Requests.Users;

public sealed class LogoutUserHttpRequest
{
    public required Guid SessionId { get; init; }
}