using Bsa.Gateway.Presentation.Http.Models.Sessions;

namespace Bsa.Gateway.Presentation.Http.Responses.Users;

public readonly record struct LoginAdminHttpResponse
{
    public required SessionBaseModel AdminSession { get; init; }
}