using Bsa.Application.Contracts.Users.User;

namespace Bsa.Presentation.Grpc.Mapping.Users.User;

public static class LoginUserRequestMappingExtensions
{
    public static LoginUser.Request MapToApplication(this ProtoLoginUserRequest protoRequest)
        => new(protoRequest.AccountNumber, protoRequest.Password);
}