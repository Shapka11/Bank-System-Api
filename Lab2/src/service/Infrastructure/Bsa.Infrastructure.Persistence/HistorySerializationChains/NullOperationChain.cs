using Bsa.Domain.HistoryOperations;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Results;
using Bsa.Infrastructure.Persistence.Models;

namespace Bsa.Infrastructure.Persistence.HistorySerializationChains;

public sealed class NullOperationChain : IHistoryOperationSerializationChain
{
    public void SetNext(IHistoryOperationSerializationChain nextChain) { }

    public SerializationHistoryOperationResult Serialize(IReadOnlyCollection<HistoryOperation> operations)
        => new SerializationHistoryOperationResult.Failure("operation for serialization not found");

    public SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation)
        => new SerializationHistoryOperationResult.Failure("operation for serialization not found");

    public DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry)
        => new DeserializationHistoryOperationResult.Failure("operation for serialization not found");
}