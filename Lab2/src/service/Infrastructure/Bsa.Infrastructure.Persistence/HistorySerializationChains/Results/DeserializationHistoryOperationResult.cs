using Bsa.Domain.HistoryOperations;

namespace Bsa.Infrastructure.Persistence.HistorySerializationChains.Results;

public abstract record DeserializationHistoryOperationResult
{
    private DeserializationHistoryOperationResult() { }

    public sealed record Success(HistoryOperation Operation) : DeserializationHistoryOperationResult;

    public sealed record Failure(string ErrorMessage) : DeserializationHistoryOperationResult;
}