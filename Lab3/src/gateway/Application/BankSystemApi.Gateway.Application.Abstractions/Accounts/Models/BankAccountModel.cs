namespace BankSystemApi.Gateway.Application.Abstractions.Accounts.Models;

public sealed record BankAccountModel(
    Guid Id,
    long UserId,
    string Number,
    string Password,
    decimal Balance,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);