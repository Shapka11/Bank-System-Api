using Bsa.Cli.Application.Abstractions.User.Models;

namespace Bsa.Cli.Infrastructure.ClientService.Users.Models;

public sealed record GetHistoryResponse(
    IReadOnlyCollection<AccountOperationEntity> History,
    string? PageToken);