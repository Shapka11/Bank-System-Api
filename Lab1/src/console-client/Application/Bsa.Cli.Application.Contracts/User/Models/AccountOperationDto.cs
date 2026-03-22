namespace Bsa.Cli.Application.Contracts.User.Models;

public sealed record AccountOperationDto(
    long Id,
    string AccountNumber,
    decimal Balance,
    string Type,
    DateTimeOffset Time);