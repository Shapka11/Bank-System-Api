using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;

namespace UnitTests.Models;

public record AccountTestModel(
    AccountId Id,
    UserId UserId,
    AccountType Type,
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
            Type,
            Number,
            Password,
            Balance,
            CreatedAt,
            UpdatedAt);
}