using Bsa.Domain.HistoryOperations;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Results;
using Bsa.Infrastructure.Persistence.Models;

namespace Bsa.Infrastructure.Persistence.HistorySerializationChains;

public abstract class HistoryOperationSerializationChain : IHistoryOperationSerializationChain
{
    protected IHistoryOperationSerializationChain NextChainElement { get; private set; } = new NullOperationChain();

    public void SetNext(IHistoryOperationSerializationChain nextChain)
    {
        NextChainElement = nextChain;
    }

    public SerializationHistoryOperationResult Serialize(IReadOnlyCollection<HistoryOperation> operations)
    {
        var payloads = new List<string>(operations.Count);

        foreach (HistoryOperation operation in operations)
        {
            SerializationHistoryOperationResult result = SerializeSingle(operation);

            if (result is SerializationHistoryOperationResult.Failure failure)
                return failure;

            if (result is SerializationHistoryOperationResult.Success success)
            {
                payloads.AddRange(success.PayloadJsons);
            }
        }

        return new SerializationHistoryOperationResult.Success(payloads);
    }

    public abstract SerializationHistoryOperationResult SerializeSingle(HistoryOperation operation);

    public abstract DeserializationHistoryOperationResult Deserialize(HistoryOperationEntry historyEntry);
}