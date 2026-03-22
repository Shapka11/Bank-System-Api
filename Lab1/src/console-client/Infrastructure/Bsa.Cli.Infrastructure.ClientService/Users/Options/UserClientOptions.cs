using System.ComponentModel.DataAnnotations;

namespace Bsa.Cli.Infrastructure.ClientService.Users.Options;

public sealed class UserClientOptions
{
    public required Uri Address { get; init; }

    [Range(minimum: 1, maximum: 100)]
    public int PageSize { get; init; }
}