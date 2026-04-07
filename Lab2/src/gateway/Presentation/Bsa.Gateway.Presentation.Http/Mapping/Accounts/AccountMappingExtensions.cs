using Bsa.Gateway.Application.Contracts.Accounts.Models;
using Bsa.Gateway.Presentation.Http.Models.Accounts;

namespace Bsa.Gateway.Presentation.Http.Mapping.Accounts;

public static class AccountMappingExtensions
{
    public static AccountModel MapToModel(this AccountDto dto)
        => new(
            dto.Id,
            dto.Number,
            dto.Password,
            dto.Balance,
            dto.CreatedAt,
            dto.UpdatedAt);
}