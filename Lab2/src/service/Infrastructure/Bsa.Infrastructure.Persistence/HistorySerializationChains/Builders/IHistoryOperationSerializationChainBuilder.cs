namespace Bsa.Infrastructure.Persistence.HistorySerializationChains.Builders;

public interface IHistoryOperationSerializationChainBuilder
{
    IHistoryOperationSerializationChainBuilder SetNext(IHistoryOperationSerializationChain nextChain);

    IHistoryOperationSerializationChain Build();
}