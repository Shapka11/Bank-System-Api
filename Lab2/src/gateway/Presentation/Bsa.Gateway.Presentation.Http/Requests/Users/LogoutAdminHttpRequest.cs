namespace Bsa.Gateway.Presentation.Http.Requests.Users;

public readonly record struct LogoutAdminHttpRequest
{
    public required Guid SessionId { get; init; }
}