namespace BankSystemApi.Gateway.Presentation.Http.Models.Accounts;

public sealed record AccountModel(
    Guid Id,
    long UserId,
    string Number,
    string Password,
    decimal Balance,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);