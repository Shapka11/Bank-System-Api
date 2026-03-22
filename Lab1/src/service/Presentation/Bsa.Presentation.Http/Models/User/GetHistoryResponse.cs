using Bsa.Application.Contracts.Models.Operations;

namespace Bsa.Presentation.Http.Models.User;

public sealed class GetHistoryResponse
{
    public required AccountOperationDto[] History { get; init; }

    public required string? PageToken { get; init; }
}