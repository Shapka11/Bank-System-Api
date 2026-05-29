using BankSystemApi.Application.Contracts.Users.Operations;

namespace BankSystemApi.Presentation.Grpc.Mapping.Users.Requests;

public static class AddUserRequestMappingExtensions
{
    public static AddUser.Request MapToApplication(this ProtoAddUserRequest protoRequest)
        => new(Guid.Parse(protoRequest.AuthorizationId));
}