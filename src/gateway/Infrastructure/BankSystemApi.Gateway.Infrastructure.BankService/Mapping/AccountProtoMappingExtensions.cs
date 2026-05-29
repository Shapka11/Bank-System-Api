using BankSystemApi.Gateway.Application.Abstractions.Accounts.Models;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Mapping;

public static class AccountProtoMappingExtensions
{
    public static BankAccountModel MapToModel(this ProtoAccount account)
        => new(
            account.Id,
            account.UserId,
            account.Type.MapToModel(),
            account.Number,
            account.Password,
            account.Balance.DecimalValue,
            account.CreatedAt.ToDateTimeOffset(),
            account.UpdatedAt.ToDateTimeOffset());
}