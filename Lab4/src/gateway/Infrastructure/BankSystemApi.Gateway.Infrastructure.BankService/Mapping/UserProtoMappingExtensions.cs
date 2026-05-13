using BankSystemApi.Gateway.Application.Abstractions.Users.Models;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Mapping;

public static class UserProtoMappingExtensions
{
    public static BankUserModel MapToModel(this ProtoUser proto)
        => new(proto.Id, Guid.Parse(proto.AuthorizationId), proto.CreatedAt.ToDateTimeOffset());
}