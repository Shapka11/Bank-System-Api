using Bsa.Domain.HistoryOperations;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Results;
using Bsa.Infrastructure.Persistence.Models;

namespace Bsa.Infrastructure.Persistence.HistorySerializationChains;

public interface IHistoryOperationSerializationChain
{
    void SetNext(IHistoryOperationSerializationChain nextChain);

    SerializationHistoryOperationResult Serialize(IReadOnlyCollection<HistoryOperation> operations);

    SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation);

    DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry);
}