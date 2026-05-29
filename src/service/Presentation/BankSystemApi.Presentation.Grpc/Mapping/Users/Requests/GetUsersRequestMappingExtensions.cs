using BankSystemApi.Application.Contracts.Users.Operations;

namespace BankSystemApi.Presentation.Grpc.Mapping.Users.Requests;

public static class GetUsersRequestMappingExtensions
{
    public static GetUsers.Request MapToApplication(this ProtoGetUsersRequest protoRequest)
        => new(
            protoRequest.AuthorizationIds.Select(Guid.Parse),
            protoRequest.UserIds,
            protoRequest.PageSize);
}