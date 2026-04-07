using Bsa.Gateway.Application.Contracts.Users.Operations;
using Bsa.Gateway.Presentation.Http.Requests.Users;

namespace Bsa.Gateway.Presentation.Http.Mapping.Users.User;

public static class LogoutUserRequestMappingExtensions
{
    public static LogoutUserRequest MapToApplication(this LogoutUserHttpRequest httpRequest)
        => new(httpRequest.SessionId);
}