namespace BankSystemApi.Domain.HistoryOperations;

public readonly record struct HistoryOperationId(long Value)
{
    public static HistoryOperationId Default => new(default);
}