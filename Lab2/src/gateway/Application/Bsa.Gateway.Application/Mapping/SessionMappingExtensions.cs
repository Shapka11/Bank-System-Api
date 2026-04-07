using Bsa.Gateway.Application.Abstractions.Users.Models;
using Bsa.Gateway.Application.Contracts.Users.Models;

namespace Bsa.Gateway.Application.Mapping;

public static class SessionMappingExtensions
{
    public static SessionBaseDto MapToDto(this BankSessionBaseModel model)
    {
        return model switch
        {
            BankAdminSessionModel bankAdminSessionModel => bankAdminSessionModel.MapToDto(),
            BankUserSessionModel bankUserSessionModel => bankUserSessionModel.MapToDto(),
            _ => throw new InvalidOperationException(),
        };
    }

    public static AdminSessionDto MapToDto(this BankAdminSessionModel model)
        => new(model.Id, model.CreatedAt);

    public static UserSessionDto MapToDto(this BankUserSessionModel model)
        => new(model.Id, model.AccountId, model.CreatedAt);
}