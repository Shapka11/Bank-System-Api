using Bsa.Gateway.Application.Contracts.Users.Operations;
using Bsa.Gateway.Presentation.Http.Requests.Users;

namespace Bsa.Gateway.Presentation.Http.Mapping.Users.User;

public static class LoginUserRequestMappingExtensions
{
    public static LoginUserRequest MapToApplication(this LoginUserHttpRequest httpRequest)
        => new(httpRequest.AccountNumber, httpRequest.Password);
}