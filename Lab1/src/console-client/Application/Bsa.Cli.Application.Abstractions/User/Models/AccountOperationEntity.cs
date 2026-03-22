namespace Bsa.Cli.Application.Abstractions.User.Models;

public sealed record AccountOperationEntity(
    long Id,
    string AccountNumber,
    decimal Balance,
    string Type,
    DateTimeOffset Time);