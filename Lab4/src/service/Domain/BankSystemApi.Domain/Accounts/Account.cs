using BankSystemApi.Domain.Accounts.Results;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;

namespace BankSystemApi.Domain.Accounts;

public sealed class Account
{
    public Account(
        AccountId id,
        UserId userId,
        AccountNumber number,
        Password password,
        Money balance,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        UserId = userId;
        Number = number;
        Password = password;
        Balance = balance;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public AccountId Id { get; }

    public UserId UserId { get; }

    public Money Balance { get; private set; }

    public AccountNumber Number { get; }

    public Password Password { get; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void Deposit(Money amount)
    {
        Balance += amount;
    }

    public WithdrawResult Withdraw(Money amount)
    {
        if (amount > Balance)
            return new WithdrawResult.Failure($"Insufficient funds. Available: {Balance}");

        Balance -= amount;

        return new WithdrawResult.Success();
    }

    public void UpdateTime(DateTimeOffset time)
    {
        UpdatedAt = time;
    }
}