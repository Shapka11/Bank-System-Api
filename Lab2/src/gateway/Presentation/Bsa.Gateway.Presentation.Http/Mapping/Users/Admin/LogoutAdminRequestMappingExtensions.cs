using Bsa.Gateway.Application.Contracts.Users.Operations;
using Bsa.Gateway.Presentation.Http.Requests.Users;

namespace Bsa.Gateway.Presentation.Http.Mapping.Users.Admin;

public static class LogoutAdminRequestMappingExtensions
{
    public static LogoutAdminRequest MapToApplication(this LogoutAdminHttpRequest httpRequest)
        => new(httpRequest.SessionId);
}