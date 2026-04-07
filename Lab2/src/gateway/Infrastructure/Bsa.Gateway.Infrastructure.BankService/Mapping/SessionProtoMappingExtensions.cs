using Bsa.Gateway.Application.Abstractions.Users.Models;

namespace Bsa.Gateway.Infrastructure.BankService.Mapping;

public static class SessionProtoMappingExtensions
{
    public static BankSessionBaseModel MapToModel(this ProtoSession proto)
    {
        return proto.SessionDataCase switch
        {
            ProtoSession.SessionDataOneofCase.Admin => MapToAdmin(proto),
            ProtoSession.SessionDataOneofCase.User => MapToUser(proto),
            ProtoSession.SessionDataOneofCase.None => throw new InvalidOperationException("Operation type is not set"),
            _ => throw new ArgumentOutOfRangeException(nameof(proto), proto, "Unknow session type"),
        };
    }

    public static BankAdminSessionModel MapToAdmin(ProtoSession proto)
        => new(Guid.Parse(proto.Id), proto.CreatedAt.ToDateTimeOffset());

    public static BankUserSessionModel MapToUser(ProtoSession proto)
        => new(Guid.Parse(proto.Id), proto.User.AccountId, proto.CreatedAt.ToDateTimeOffset());
}