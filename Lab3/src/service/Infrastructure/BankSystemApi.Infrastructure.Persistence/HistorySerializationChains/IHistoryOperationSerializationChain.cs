using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Results;
using BankSystemApi.Infrastructure.Persistence.Models;

namespace BankSystemApi.Infrastructure.Persistence.HistorySerializationChains;

public interface IHistoryOperationSerializationChain
{
    void SetNext(IHistoryOperationSerializationChain nextChain);

    SerializationHistoryOperationResult Serialize(IReadOnlyCollection<HistoryOperation> operations);

    SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation);

    DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry);
}