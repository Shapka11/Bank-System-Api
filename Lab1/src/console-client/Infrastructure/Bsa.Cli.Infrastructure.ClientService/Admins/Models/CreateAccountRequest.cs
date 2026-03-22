namespace Bsa.Cli.Infrastructure.ClientService.Admins.Models;

public sealed record CreateAccountRequest(Guid Id, string AccountNumber, string Password);