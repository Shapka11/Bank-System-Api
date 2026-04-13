using Bsa.Application.Contracts.Accounts.Models;
using Bsa.Domain.Accounts;

namespace Bsa.Application.Mapping;

public static class AccountMappingExtensions
{
    public static AccountDto MapToDto(this Account account)
        => new(
            account.Id.Value,
            account.Number.Value,
            account.Password.Value,
            account.Balance.Value,
            account.CreatedAt,
            account.UpdatedAt);
}