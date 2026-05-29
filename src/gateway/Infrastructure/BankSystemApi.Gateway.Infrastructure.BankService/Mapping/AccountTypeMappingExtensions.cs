using BankSystemApi.Gateway.Application.Abstractions.Accounts.Models;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Mapping;

public static class AccountTypeMappingExtensions
{
    public static BankAccountTypeModel MapToModel(this ProtoAccountType protoType)
    {
        return protoType switch
        {
            ProtoAccountType.Unspecified => throw new ArgumentException("Type is not set", nameof(protoType)),
            ProtoAccountType.Personal => BankAccountTypeModel.Personal,
            ProtoAccountType.Corporate => BankAccountTypeModel.Corporate,
            _ => throw new ArgumentOutOfRangeException(nameof(protoType), protoType, "Incorrect type"),
        };
    }

    public static ProtoAccountType MapToProto(this BankAccountTypeModel model)
    {
        return model switch
        {
            BankAccountTypeModel.Personal => ProtoAccountType.Personal,
            BankAccountTypeModel.Corporate => ProtoAccountType.Corporate,
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, "Incorrect type"),
        };
    }
}