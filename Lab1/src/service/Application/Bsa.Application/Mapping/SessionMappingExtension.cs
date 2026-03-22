using Bsa.Application.Contracts.Models.Sessions;
using Bsa.Domain.Sessions;

namespace Bsa.Application.Mapping;

public static class SessionMappingExtension
{
    public static UserSessionDto MapToDto(this UserSession session)
        => new UserSessionDto(session.Id, session.AccountId.Value);

    public static AdminSessionDto MapToDto(this AdminSession session)
        => new AdminSessionDto(session.Id);
}