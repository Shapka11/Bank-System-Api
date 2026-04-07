namespace Bsa.Gateway.Presentation.Http.Models.Accounts;

public sealed record AccountModel(
    long Id,
    string Number,
    string Password,
    decimal Balance,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);