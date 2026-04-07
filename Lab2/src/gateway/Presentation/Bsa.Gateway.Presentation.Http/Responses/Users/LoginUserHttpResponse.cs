using Bsa.Gateway.Presentation.Http.Models.Sessions;

namespace Bsa.Gateway.Presentation.Http.Responses.Users;

public readonly record struct LoginUserHttpResponse
{
    public required SessionBaseModel UserSession { get; init; }
}