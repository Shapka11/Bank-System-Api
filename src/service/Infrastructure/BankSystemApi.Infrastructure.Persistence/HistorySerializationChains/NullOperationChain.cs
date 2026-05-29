using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Results;
using BankSystemApi.Infrastructure.Persistence.Models;

namespace BankSystemApi.Infrastructure.Persistence.HistorySerializationChains;

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