using BankSystemApi.Application.Abstractions.Events.Models;
using BankSystemApi.Application.Contracts.Accounts.Models;
using BankSystemApi.Domain.Accounts;

namespace BankSystemApi.Application.Mapping;

public static class AccountTypeMappingExtensions
{
    public static AccountTypeDto MapToDto(this AccountType type)
    {
        return type switch
        {
            AccountType.Personal => AccountTypeDto.Personal,
            AccountType.Corporate => AccountTypeDto.Corporate,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Incorrect type"),
        };
    }

    public static AccountType MapToDomain(this AccountTypeDto dto)
    {
        return dto switch
        {
            AccountTypeDto.Personal => AccountType.Personal,
            AccountTypeDto.Corporate => AccountType.Corporate,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, "Incorrect type"),
        };
    }

    public static CreationAccountType MapToEvent(this AccountType type)
    {
        return type switch
        {
            AccountType.Personal => CreationAccountType.Personal,
            AccountType.Corporate => CreationAccountType.Corporate,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Incorrect type"),
        };
    }
}