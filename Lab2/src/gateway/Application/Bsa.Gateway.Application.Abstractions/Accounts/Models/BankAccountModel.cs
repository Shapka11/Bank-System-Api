namespace Bsa.Gateway.Application.Abstractions.Accounts.Models;

public sealed record BankAccountModel(
    long Id,
    string Number,
    string Password,
    decimal Balance,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);