using System.ComponentModel.DataAnnotations;

namespace Bsa.Presentation.Http.Models.User;

public sealed class GetHistoryRequest
{
    public required Guid Id { get; init; }

    public string? PageToken { get; init; }

    [Range(minimum: 1, maximum: 1000)]
    public int PageSize { get; init; }
}