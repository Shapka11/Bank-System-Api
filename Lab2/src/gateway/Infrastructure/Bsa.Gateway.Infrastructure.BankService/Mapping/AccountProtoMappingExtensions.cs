using Bsa.Gateway.Application.Abstractions.Accounts.Models;

namespace Bsa.Gateway.Infrastructure.BankService.Mapping;

public static class AccountProtoMappingExtensions
{
    public static BankAccountModel MapToModel(this ProtoAccount account)
        => new(
            account.Id,
            account.Number,
            account.Password,
            account.Balance.DecimalValue,
            account.CreatedAt.ToDateTimeOffset(),
            account.UpdatedAt.ToDateTimeOffset());
}