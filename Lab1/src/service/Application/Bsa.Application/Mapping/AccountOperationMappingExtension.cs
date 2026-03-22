using Bsa.Application.Contracts.Models.Operations;
using Bsa.Domain.Operations;

namespace Bsa.Application.Mapping;

public static class AccountOperationMappingExtension
{
    public static AccountOperationDto MapToDto(this AccountOperation operation)
        => new AccountOperationDto(
            operation.Id.Value,
            operation.Number.Value,
            operation.Balance.Value,
            operation.OperationType.ToString(),
            operation.CreatedTime);

    public static AccountOperationDto[] MapToDto(this AccountOperation[] operations)
        => operations.Select(operation => operation.MapToDto()).ToArray();
}