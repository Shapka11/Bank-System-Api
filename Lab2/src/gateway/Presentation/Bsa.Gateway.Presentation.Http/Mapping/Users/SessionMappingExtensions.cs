using Bsa.Gateway.Application.Contracts.Users.Models;
using Bsa.Gateway.Presentation.Http.Models.Sessions;

namespace Bsa.Gateway.Presentation.Http.Mapping.Users;

public static class SessionMappingExtensions
{
    public static SessionBaseModel MapToModel(this SessionBaseDto dto)
    {
        return dto switch
        {
            AdminSessionDto adminSessionDto => adminSessionDto.MapToModel(),
            UserSessionDto userSessionDto => userSessionDto.MapToModel(),
            _ => throw new InvalidOperationException($"Unknown DTO type: {dto.GetType().Name}"),
        };
    }

    public static AdminSessionModel MapToModel(this AdminSessionDto dto)
        => new(dto.Id, dto.CreatedAt);

    public static UserSessionModel MapToModel(this UserSessionDto dto)
        => new(dto.Id, dto.AccountId, dto.CreatedAt);
}