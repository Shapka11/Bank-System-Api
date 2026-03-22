using Bsa.Application.Contracts.Models.Accounts;
using Bsa.Domain.Accounts;

namespace Bsa.Application.Mapping;

public static class AccountMappingExtension
{
    public static AccountDto MapToDto(this Account account)
        => new AccountDto(account.Number.Value, account.Balance.Value);
}