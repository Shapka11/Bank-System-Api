using BankSystemApi.Gateway.Application.Contracts.Accounts.Models;
using BankSystemApi.Gateway.Presentation.Http.Models.Accounts;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.Accounts;

public static class AccountTypeMappingExtensions
{
    public static AccountTypeModel MapToModel(this AccountTypeDto dto)
    {
        return dto switch
        {
            AccountTypeDto.Personal => AccountTypeModel.Personal,
            AccountTypeDto.Corporate => AccountTypeModel.Corporate,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, "Incorrect type"),
        };
    }

    public static AccountTypeDto MapToDto(this AccountTypeModel model)
    {
        return model switch
        {
            AccountTypeModel.Personal => AccountTypeDto.Personal,
            AccountTypeModel.Corporate => AccountTypeDto.Corporate,
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, "Incorrect type"),
        };
    }
}