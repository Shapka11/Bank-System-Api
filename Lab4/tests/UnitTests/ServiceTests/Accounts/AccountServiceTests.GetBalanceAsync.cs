using AutoBogus;
using BankSystemApi.Application.Contracts.Accounts.Operations;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using BankSystemApi.Domain.Users;
using FluentAssertions;
using UnitTests.MockExtensions;
using UnitTests.Models;

namespace UnitTests.ServiceTests.Accounts;

public sealed partial class AccountServiceTests
{
    [Fact]
    public async Task GetBalanceAsync_ShouldReturnBalance_WhenAccountExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account account = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();

        GetBalance.Request request = new(user.AuthorizationId, account.Id.Value);

        DateTimeOffset currentTime = _faker.Date.RecentOffset();
        _dateTimeProvider.Setup(time => time.Current).Returns(currentTime);

        var historyOperation = new CheckBalanceHistoryOperation(
            HistoryOperationId.Default,
            account.Id,
            account.Balance,
            currentTime);
        var expectedHistoryOperationId = new HistoryOperationId(1);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(account.Id, account);

        _persistenceContext.HistoryOperationsRepository
            .SetupAddHistoryOperation([historyOperation], [expectedHistoryOperationId]);

        // Act
        GetBalance.Response response = await _accountService.GetBalanceAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<GetBalance.Response.Success>()
            .Which.Balance.Should()
            .Be(account.Balance.Value);
    }

    [Fact]
    public async Task GetBalanceAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        var accountId = new AccountId(_faker.Random.Guid());
        GetBalance.Request request = new(user.AuthorizationId, accountId.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId);

        // Act
        GetBalance.Response response = await _accountService.GetBalanceAsync(request, default);

        // Assert
        response.Should().BeOfType<GetBalance.Response.Unauthorized>();
    }

    [Fact]
    public async Task GetBalanceAsync_ShouldReturnAccountNotFound_WhenAccountNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        var accountId = new AccountId(_faker.Random.Guid());
        GetBalance.Request request = new(user.AuthorizationId, accountId.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(accountId);

        // Act
        GetBalance.Response response = await _accountService.GetBalanceAsync(request, default);

        // Assert
        response.Should().BeOfType<GetBalance.Response.AccountNotFound>();
    }

    [Fact]
    public async Task GetBalanceAsync_ShouldReturnForbidden_WhenAccountNotUsers()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account account = new AutoFaker<Account>().Generate();
        GetBalance.Request request = new(user.AuthorizationId, account.Id.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(account.Id, account);

        // Act
        GetBalance.Response response = await _accountService.GetBalanceAsync(request, default);

        // Assert
        response.Should().BeOfType<GetBalance.Response.Forbidden>();
    }
}