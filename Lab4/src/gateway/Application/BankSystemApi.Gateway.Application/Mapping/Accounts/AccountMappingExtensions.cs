using BankSystemApi.Gateway.Application.Abstractions.Accounts.Models;
using BankSystemApi.Gateway.Application.Contracts.Accounts.Models;

namespace BankSystemApi.Gateway.Application.Mapping.Accounts;

public static class AccountMappingExtensions
{
    public static AccountDto MapToDto(this BankAccountModel model)
        => new(
            model.Id,
            model.UserId,
            model.Number,
            model.Password,
            model.Balance,
            model.CreatedAt,
            model.UpdatedAt);
}