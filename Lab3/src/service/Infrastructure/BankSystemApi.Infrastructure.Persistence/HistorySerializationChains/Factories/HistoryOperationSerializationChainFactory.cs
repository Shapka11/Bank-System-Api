using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Accounts;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Builders;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Invoices;

namespace BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Factories;

public sealed class HistoryOperationSerializationChainFactory
{
    private readonly IHistoryOperationSerializationChainBuilder _builder;

    public HistoryOperationSerializationChainFactory(IHistoryOperationSerializationChainBuilder builder)
    {
        _builder = builder;
    }

    public IHistoryOperationSerializationChain Create() => _builder
        .SetNext(new CreateAccountHistoryOperationChain())
        .SetNext(new CheckBalanceHistoryOperationChain())
        .SetNext(new DepositHistoryOperationChain())
        .SetNext(new WithdrawHistoryOperationChain())
        .SetNext(new InvoiceIssuedHistoryOperationChain())
        .SetNext(new InvoiceReceivedHistoryOperationChain())
        .SetNext(new InvoicePaymentReceivedHistoryOperationChain())
        .SetNext(new InvoicePaymentSentHistoryOperationChain())
        .SetNext(new InvoiceRevokedHistoryOperationChain())
        .SetNext(new NullOperationChain())
        .Build();
}