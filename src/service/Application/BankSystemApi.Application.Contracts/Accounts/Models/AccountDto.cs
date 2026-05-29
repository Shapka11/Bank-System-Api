namespace BankSystemApi.Application.Contracts.Accounts.Models;

public sealed record AccountDto(
    long Id,
    long UserId,
    AccountTypeDto Type,
    string Number,
    string Password,
    decimal Balance,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);