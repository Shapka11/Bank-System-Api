using BankSystemApi.Application.Contracts.Users.Models;
using Google.Protobuf.WellKnownTypes;

namespace BankSystemApi.Presentation.Grpc.Mapping.Users;

public static class UserProtoMappingExtensions
{
    public static ProtoUser MapToProto(this UserDto user)
        => new(user.Id, user.AutorizationId.ToString(), user.CreatedAt.ToTimestamp());
}