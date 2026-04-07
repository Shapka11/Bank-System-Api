namespace Bsa.Gateway.Presentation.Http.Requests.Users;

public readonly record struct LogoutUserHttpRequest
{
    public required Guid SessionId { get; init; }
}