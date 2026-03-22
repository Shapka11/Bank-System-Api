using Bsa.Cli.Application.Abstractions.User.Models;
using Bsa.Cli.Application.Contracts.User.Models;

namespace Bsa.Cli.Application.Mapping;

public static class AccountOperationMappingExtension
{
    public static AccountOperationDto MapToDto(this AccountOperationEntity operation)
        => new AccountOperationDto(
            operation.Id,
            operation.AccountNumber,
            operation.Balance,
            operation.Type,
            operation.Time);

    public static IEnumerable<AccountOperationDto> MapToDto(this IEnumerable<AccountOperationEntity> operations)
        => operations.Select(o => o.MapToDto());
}