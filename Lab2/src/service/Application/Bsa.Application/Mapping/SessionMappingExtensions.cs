using Bsa.Application.Contracts.Users.Models;
using Bsa.Domain.Sessions;

namespace Bsa.Application.Mapping;

public static class SessionMappingExtensions
{
    public static SessionBaseDto MapToDto(this SessionBase session)
    {
        return session switch
        {
            AdminSession adminSession => adminSession.MapToDto(),
            UserSession userSession => userSession.MapToDto(),
            _ => throw new ArgumentOutOfRangeException(
                nameof(session),
                $"Mapping not supported for {session.GetType().Name}"),
        };
    }

    public static UserSessionDto MapToDto(this UserSession session)
        => new(session.Id, session.AccountId.Value, session.CreatedAt);

    public static AdminSessionDto MapToDto(this AdminSession session)
        => new(session.Id, session.CreatedAt);
}