namespace Bsa.Gateway.Presentation.Http.Requests.Users;

public sealed class LogoutAdminHttpRequest
{
    public required Guid SessionId { get; init; }
}