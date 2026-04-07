namespace Bsa.Infrastructure.Persistence.HistorySerializationChains.Builders;

public sealed class HistoryOperationSerializationChainBuilder : IHistoryOperationSerializationChainBuilder
{
    private readonly List<IHistoryOperationSerializationChain> _operations = [];

    public IHistoryOperationSerializationChainBuilder SetNext(IHistoryOperationSerializationChain nextChain)
    {
        _operations.Add(nextChain);
        return this;
    }

    public IHistoryOperationSerializationChain Build()
    {
        if (_operations.Count == 0)
            throw new InvalidOperationException("no arguments");

        for (int i = 0; i < _operations.Count - 1; ++i)
        {
            _operations[i].SetNext(_operations[i + 1]);
        }

        return _operations[0];
    }
}