using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Application.Abstractions.Persistence.Results;
using BankSystemApi.Domain.Users;
using Moq;

namespace UnitTests.MockExtensions;

public static class UserRepositoryMockExtensions
{
    public static Mock<IUserRepository> SetupQueryUserByAuthId(
        this Mock<IUserRepository> mock,
        Guid authId,
        params User[] returnedUsers)
    {
        mock
            .Setup(repo => repo.QueryAsync(
                It.Is<UserQuery>(q => q.AuthorizationIds.Contains(authId)),
                It.IsAny<CancellationToken>()))
            .Returns(returnedUsers.ToAsyncEnumerable);

        return mock;
    }

    public static Mock<IUserRepository> SetupQueryUserById(
        this Mock<IUserRepository> mock,
        UserId userId,
        params User[] returnedUsers)
    {
        mock
            .Setup(repo => repo.QueryAsync(
                It.Is<UserQuery>(q => q.Ids.Contains(userId)),
                It.IsAny<CancellationToken>()))
            .Returns(returnedUsers.ToAsyncEnumerable);

        return mock;
    }

    public static Mock<IUserRepository> SetupTryAddUser(
        this Mock<IUserRepository> mock,
        User user,
        AddUserResult expectedResult)
    {
        mock
            .Setup(repo => repo.TryAddAsync(
                It.Is<IReadOnlyCollection<User>>(c => c.Any(u => u.AuthorizationId == user.AuthorizationId)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        return mock;
    }
}