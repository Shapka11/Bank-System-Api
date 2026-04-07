using Bsa.Application.Contracts.Users.Admin;

namespace Bsa.Presentation.Grpc.Mapping.Users.Admin;

public static class LogoutAdminRequestMappingExtensions
{
    public static LogoutAdmin.Request MapToApplication(this ProtoLogoutAdminRequest protoRequest)
        => new(Guid.Parse(protoRequest.Id));
}