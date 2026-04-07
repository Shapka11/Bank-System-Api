using Bsa.Application.Contracts.Users.User;

namespace Bsa.Presentation.Grpc.Mapping.Users.User;

public static class LogoutUserRequestMappingExtensions
{
    public static LogoutUser.Request MapToApplication(this ProtoLogoutUserRequest protoRequest)
        => new(Guid.Parse(protoRequest.Id));
}