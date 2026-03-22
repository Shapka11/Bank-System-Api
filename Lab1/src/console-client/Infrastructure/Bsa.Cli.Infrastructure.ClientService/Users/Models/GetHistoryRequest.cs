namespace Bsa.Cli.Infrastructure.ClientService.Users.Models;

public sealed record GetHistoryRequest(Guid Id, string? PageToken, int PageSize);