namespace BankSystemApi.Gateway.Application.Contracts.Accounts.Models;

public sealed record AccountDto(
    Guid Id,
    long UserId,
    string Number,
    string Password,
    decimal Balance,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);