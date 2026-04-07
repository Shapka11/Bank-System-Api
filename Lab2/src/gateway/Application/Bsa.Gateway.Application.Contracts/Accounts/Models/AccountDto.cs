namespace Bsa.Gateway.Application.Contracts.Accounts.Models;

public sealed record AccountDto(
    long Id,
    string Number,
    string Password,
    decimal Balance,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);