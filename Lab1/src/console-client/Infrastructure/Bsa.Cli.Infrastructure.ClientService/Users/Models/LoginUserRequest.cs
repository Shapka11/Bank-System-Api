namespace Bsa.Cli.Infrastructure.ClientService.Users.Models;

public sealed record LoginUserRequest(string AccountNumber, string Password);