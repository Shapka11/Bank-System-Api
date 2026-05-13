using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using Moq;

namespace UnitTests.MockExtensions;

public static class HistoryOperationMockExtensions
{
    public static Mock<IHistoryOperationRepository> SetupAddHistoryOperation(
        this Mock<IHistoryOperationRepository> mock,
        HistoryOperation[] operations,
        HistoryOperationId[] expectedIds)
    {
        mock
            .Setup(repo => repo.AddAsync(
                It.Is<IReadOnlyCollection<HistoryOperation>>(actualList =>
                    actualList.Count == operations.Length &&
                    actualList.All(actual => operations.Any(exp => exp.AccountId == actual.AccountId))),
                It.IsAny<CancellationToken>()))
            .Returns((IReadOnlyCollection<HistoryOperation> incomingList, CancellationToken _) =>
            {
                IEnumerable<HistoryOperation> resultWithIds = incomingList.Zip(
                    expectedIds,
                    (operation, id) => operation with { Id = id });

                return resultWithIds.ToAsyncEnumerable();
            });

        return mock;
    }

    public static Mock<IHistoryOperationRepository> SetupQueryHistoryOperationByAccountId(
        this Mock<IHistoryOperationRepository> mock,
        AccountId id,
        params CreateAccountHistoryOperation[] returnedOperations)
    {
        mock
            .Setup(repo => repo.QueryAsync(
                It.Is<HistoryOperationQuery>(q => q.AccountIds.Contains(id)),
                It.IsAny<CancellationToken>()))
            .Returns(returnedOperations.ToAsyncEnumerable());

        return mock;
    }
}