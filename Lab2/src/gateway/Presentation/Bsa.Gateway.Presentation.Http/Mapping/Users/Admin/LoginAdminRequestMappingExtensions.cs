using Bsa.Gateway.Application.Contracts.Users.Operations;
using Bsa.Gateway.Presentation.Http.Requests.Users;

namespace Bsa.Gateway.Presentation.Http.Mapping.Users.Admin;

public static class LoginAdminRequestMappingExtensions
{
    public static LoginAdminRequest MapToApplication(this LoginAdminHttpRequest httpRequest)
        => new(httpRequest.Password);
}