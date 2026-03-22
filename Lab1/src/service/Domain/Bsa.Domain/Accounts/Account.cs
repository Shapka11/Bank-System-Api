using Bsa.Domain.Accounts.Results;
using Bsa.Domain.ValueObjects;

namespace Bsa.Domain.Accounts;

public sealed class Account
{
    public Account(
        AccountId id,
        AccountNumber number,
        Password password,
        Money balance,
        DateTimeOffset createdTime,
        DateTimeOffset updatedTime)
    {
        Id = id;
        Number = number;
        Password = password;
        Balance = balance;
        CreatedTime = createdTime;
        UpdatedTime = updatedTime;
    }

    public AccountId Id { get; }

    public Money Balance { get; private set; }

    public AccountNumber Number { get; }

    public Password Password { get; }

    public DateTimeOffset CreatedTime { get; }

    public DateTimeOffset UpdatedTime { get; private set; }

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

    public bool VerifyPassword(Password password) => Password == password;

    public void UpdateTime()
    {
        UpdatedTime = DateTimeOffset.UtcNow;
    }
}