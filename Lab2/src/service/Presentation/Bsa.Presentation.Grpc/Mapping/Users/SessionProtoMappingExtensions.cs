using Bsa.Application.Contracts.Users.Models;
using Google.Protobuf.WellKnownTypes;

namespace Bsa.Presentation.Grpc.Mapping.Users;

public static class SessionProtoMappingExtensions
{
    public static ProtoSession MapToProto(this SessionBaseDto dto)
    {
        var proto = new ProtoSession
        {
            Id = dto.Id.ToString(),
            CreatedAt = dto.CreatedAt.ToTimestamp(),
        };

        switch (dto)
        {
            case AdminSessionDto d: proto.Admin = d.MapToData(); break;
            case UserSessionDto d: proto.User = d.MapToData(); break;
            default: throw new NotSupportedException($"Type {dto.GetType().Name} is not supported");
        }

        return proto;
    }

    public static ProtoSession.Types.UserData MapToData(this UserSessionDto dto)
        => new() { AccountId = dto.AccountId };

    public static ProtoSession.Types.AdminData MapToData(this AdminSessionDto dto) => new();
}