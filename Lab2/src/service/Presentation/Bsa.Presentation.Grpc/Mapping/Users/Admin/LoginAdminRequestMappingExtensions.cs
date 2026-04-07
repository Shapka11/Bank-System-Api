using Bsa.Application.Contracts.Users.Admin;

namespace Bsa.Presentation.Grpc.Mapping.Users.Admin;

public static class LoginAdminRequestMappingExtensions
{
    public static LoginAdmin.Request MapToApplication(this ProtoLoginAdminRequest protoRequest)
        => new(protoRequest.Password);
}