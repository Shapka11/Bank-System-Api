namespace Bsa.Infrastructure.Persistence.HistorySerializationChains.Results;

public abstract record SerializationHistoryOperationResult
{
    private SerializationHistoryOperationResult() { }

    public sealed record Success(IReadOnlyCollection<string> PayloadJsons) : SerializationHistoryOperationResult;

    public sealed record Failure(string ErrorMessage) : SerializationHistoryOperationResult;
}