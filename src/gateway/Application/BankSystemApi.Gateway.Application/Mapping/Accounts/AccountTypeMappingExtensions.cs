using BankSystemApi.Gateway.Application.Abstractions.Accounts.Models;
using BankSystemApi.Gateway.Application.Contracts.Accounts.Models;

namespace BankSystemApi.Gateway.Application.Mapping.Accounts;

public static class AccountTypeMappingExtensions
{
    public static BankAccountTypeModel MapToBankModel(this AccountTypeDto dto)
    {
        return dto switch
        {
            AccountTypeDto.Personal => BankAccountTypeModel.Personal,
            AccountTypeDto.Corporate => BankAccountTypeModel.Corporate,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, "Incorrect type"),
        };
    }

    public static AccountTypeDto MapToDto(this BankAccountTypeModel model)
    {
        return model switch
        {
            BankAccountTypeModel.Personal => AccountTypeDto.Personal,
            BankAccountTypeModel.Corporate => AccountTypeDto.Corporate,
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, "Incorrect type"),
        };
    }
}