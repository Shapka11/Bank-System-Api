using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Moq;
using System.Data;

namespace UnitTests.MockExtensions;

public static class MockTransactionExtensions
{
    public static void SetupDefaultTransaction(this Mock<IPersistenceTransactionProvider> providerMock)
    {
        var transactionMock = new Mock<IPersistenceTransaction>(MockBehavior.Strict);
        transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(ValueTask.CompletedTask);
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        providerMock
            .Setup(p => p.BeginTransactionAsync(It.IsAny<IsolationLevel>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<IPersistenceTransaction>(transactionMock.Object));
    }
}