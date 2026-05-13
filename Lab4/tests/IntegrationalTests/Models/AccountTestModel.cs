using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;

namespace IntegrationalTests.Models;

public record AccountTestModel(
    AccountId Id,
    UserId UserId,
    AccountNumber Number,
    Password Password,
    Money Balance,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public Account MapToDomain()
        => new(
            Id,
            UserId,
            Number,
            Password,
            Balance,
            CreatedAt,
            UpdatedAt);
}