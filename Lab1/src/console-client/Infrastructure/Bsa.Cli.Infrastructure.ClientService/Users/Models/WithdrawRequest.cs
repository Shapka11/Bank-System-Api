namespace Bsa.Cli.Infrastructure.ClientService.Users.Models;

public sealed record WithdrawRequest(Guid Id, decimal Amount);