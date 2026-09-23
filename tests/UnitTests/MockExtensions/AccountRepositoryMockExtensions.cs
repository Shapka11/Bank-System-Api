using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Application.Abstractions.Persistence.Results;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using Moq;

namespace UnitTests.MockExtensions;

public static class AccountRepositoryMockExtensions
{
    public static Mock<IAccountRepository> SetupTryAddAccount(
        this Mock<IAccountRepository> mock,
        Account account,
        AddAccountResult expectedResult)
    {
        mock
            .Setup(repo => repo.TryAddAsync(
                It.Is<Account>(candidate => candidate.Number == account.Number),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        return mock;
    }

    public static Mock<IAccountRepository> SetupGetAccountsByIdsForUpdate(
        this Mock<IAccountRepository> mock,
        AccountId accountId,
        params Account[] returnedAccounts)
        => mock.SetupGetAccountsByIdsForUpdate([accountId], returnedAccounts);

    public static Mock<IAccountRepository> SetupGetAccountsByIdsForUpdate(
        this Mock<IAccountRepository> mock,
        IReadOnlyCollection<AccountId> accountIds,
        params Account[] returnedAccounts)
    {
        mock
            .Setup(repo => repo.GetByIdsForUpdateAsync(
                It.Is<IReadOnlyCollection<AccountId>>(ids =>
                    ids.Count == accountIds.Count && accountIds.All(ids.Contains)),
                It.IsAny<CancellationToken>()))
            .Returns(returnedAccounts.ToAsyncEnumerable());

        return mock;
    }

    public static Mock<IAccountRepository> SetupUpdateAccount(
        this Mock<IAccountRepository> mock,
        Account[] accounts)
    {
        mock
            .Setup(repo => repo.UpdateAsync(
                It.Is<IReadOnlyCollection<Account>>(queryAccounts =>
                    queryAccounts.Count == accounts.Length &&
                    accounts.All(a => queryAccounts.Any(q => q.Number == a.Number))),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mock;
    }

    public static Mock<IAccountRepository> SetupQueryAccountByUserId(
        this Mock<IAccountRepository> mock,
        UserId userId,
        params Account[] returnedAccounts)
    {
        mock
            .Setup(repo => repo.QueryAsync(
                It.Is<AccountQuery>(q => q.UserIds.Contains(userId)),
                It.IsAny<CancellationToken>()))
            .Returns(returnedAccounts.ToAsyncEnumerable());

        return mock;
    }

    public static Mock<IAccountRepository> SetupQueryAccountByNumber(
        this Mock<IAccountRepository> mock,
        AccountNumber number,
        params Account[] returnedAccounts)
    {
        mock
            .Setup(repo => repo.QueryAsync(
                It.Is<AccountQuery>(q => q.AccountNumbers.Contains(number)),
                It.IsAny<CancellationToken>()))
            .Returns(returnedAccounts.ToAsyncEnumerable());

        return mock;
    }

    public static Mock<IAccountRepository> SetupQueryAccountById(
        this Mock<IAccountRepository> mock,
        AccountId id,
        params Account[] returnedAccounts)
    {
        mock
            .Setup(repo => repo.QueryAsync(
                It.Is<AccountQuery>(q => q.AccountIds.Contains(id)),
                It.IsAny<CancellationToken>()))
            .Returns(returnedAccounts.ToAsyncEnumerable());

        return mock;
    }
}
