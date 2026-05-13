using BankSystemApi.Gateway.Application.Contracts.Accounts.Models;
using BankSystemApi.Gateway.Presentation.Http.Models.Accounts;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.Accounts;

public static class AccountMappingExtensions
{
    public static AccountModel MapToModel(this AccountDto dto)
        => new(
            dto.Id,
            dto.UserId,
            dto.Number,
            dto.Password,
            dto.Balance,
            dto.CreatedAt,
            dto.UpdatedAt);
}