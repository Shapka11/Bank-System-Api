using BankSystemApi.Application.Contracts.Accounts.Models;
using BankSystemApi.Domain.Accounts;

namespace BankSystemApi.Application.Mapping;

public static class AccountMappingExtensions
{
    public static AccountDto MapToDto(this Account account)
        => new(
            account.Id.Value,
            account.UserId.Value,
            account.Type.MapToDto(),
            account.Number.Value,
            account.Password.Value,
            account.Balance.Value,
            account.CreatedAt,
            account.UpdatedAt);
}