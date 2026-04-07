using Bsa.Gateway.Application.Abstractions.Accounts.Models;
using Bsa.Gateway.Application.Contracts.Accounts.Models;

namespace Bsa.Gateway.Application.Mapping;

public static class AccountMappingExtensions
{
    public static AccountDto MapToDto(this BankAccountModel model)
        => new AccountDto(
            model.Id,
            model.Number,
            model.Password,
            model.Balance,
            model.CreatedAt,
            model.UpdatedAt);
}