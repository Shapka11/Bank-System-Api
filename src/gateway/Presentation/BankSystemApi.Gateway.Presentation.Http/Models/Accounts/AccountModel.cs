namespace BankSystemApi.Gateway.Presentation.Http.Models.Accounts;

public sealed record AccountModel(
    long Id,
    long UserId,
    AccountTypeModel Type,
    string Number,
    string Password,
    decimal Balance,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);