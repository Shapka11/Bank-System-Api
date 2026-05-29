using BankSystemApi.Application.Contracts.Accounts.Models;

namespace BankSystemApi.Presentation.Grpc.Mapping.Accounts;

public static class AccountTypeMappingExtensions
{
    public static ProtoAccountType MapToProto(this AccountTypeDto dto)
    {
        return dto switch
        {
            AccountTypeDto.Personal => ProtoAccountType.Personal,
            AccountTypeDto.Corporate => ProtoAccountType.Corporate,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, "Incorrect type"),
        };
    }

    public static AccountTypeDto MapToDto(this ProtoAccountType protoType)
    {
        return protoType switch
        {
            ProtoAccountType.Unspecified => throw new ArgumentException("Type is not set", nameof(protoType)),
            ProtoAccountType.Personal => AccountTypeDto.Personal,
            ProtoAccountType.Corporate => AccountTypeDto.Corporate,
            _ => throw new ArgumentOutOfRangeException(nameof(protoType), protoType, "Incorrect type"),
        };
    }
}