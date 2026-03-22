namespace Bsa.Domain.Operations;

public readonly record struct AccountOperationId(long Value)
{
    public static AccountOperationId Default => new(default);
}