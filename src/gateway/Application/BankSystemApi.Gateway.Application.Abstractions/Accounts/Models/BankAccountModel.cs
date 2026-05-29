namespace BankSystemApi.Gateway.Application.Abstractions.Accounts.Models;

public sealed record BankAccountModel(
    long Id,
    long UserId,
    BankAccountTypeModel Type,
    string Number,
    string Password,
    decimal Balance,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);