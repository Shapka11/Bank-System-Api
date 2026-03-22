using Bsa.Domain.Accounts;
using Bsa.Domain.ValueObjects;

namespace Bsa.Domain.Operations;

public sealed record AccountOperation
{
    public AccountOperation(
        AccountOperationId id,
        AccountId accountId,
        AccountNumber number,
        Money balance,
        AccountOperationType operationType,
        DateTimeOffset createdTime)
    {
        Id = id;
        AccountId = accountId;
        Number = number;
        Balance = balance;
        OperationType = operationType;
        CreatedTime = createdTime;
    }

    public AccountOperationId Id { get; }

    public AccountId AccountId { get; }

    public AccountNumber Number { get; }

    public Money Balance { get; }

    public AccountOperationType OperationType { get; }

    public DateTimeOffset CreatedTime { get; }
}