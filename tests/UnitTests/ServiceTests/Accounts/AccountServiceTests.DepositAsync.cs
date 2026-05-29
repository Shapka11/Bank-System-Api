using AutoBogus;
using BankSystemApi.Application.Contracts.Accounts.Models;
using BankSystemApi.Application.Contracts.Accounts.Operations;
using BankSystemApi.Application.Mapping;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using FluentAssertions;
using UnitTests.MockExtensions;
using UnitTests.Models;

namespace UnitTests.ServiceTests.Accounts;

public sealed partial class AccountServiceTests
{
    [Fact]
    public async Task DepositAsync_ShouldReturnDto_WhenAccountExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account account = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();
        var amount = new Money(_faker.Finance.Amount());

        Deposit.Request request = new(
            user.AuthorizationId,
            account.Id.Value,
            amount.Value);

        DateTimeOffset updatedAccountTime = _faker.Date.RecentOffset();
        _dateTimeProvider.Setup(time => time.Current).Returns(updatedAccountTime);

        var updatedAccount = new Account(
            account.Id,
            account.UserId,
            account.Type,
            account.Number,
            account.Password,
            account.Balance + amount,
            account.CreatedAt,
            updatedAccountTime);

        var historyOperation = new DepositHistoryOperation(
            HistoryOperationId.Default,
            account.Id,
            amount,
            updatedAccountTime);
        var expectedHistoryOperationId = new HistoryOperationId(1);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(account.Id, account)
            .SetupUpdateAccount([updatedAccount]);

        _persistenceContext.HistoryOperationsRepository
            .SetupAddHistoryOperation([historyOperation], [expectedHistoryOperationId]);

        var accountDto = new AccountDto(
            updatedAccount.Id.Value,
            updatedAccount.UserId.Value,
            updatedAccount.Type.MapToDto(),
            updatedAccount.Number.Value,
            updatedAccount.Password.Value,
            updatedAccount.Balance.Value,
            updatedAccount.CreatedAt,
            updatedAccount.UpdatedAt);

        _serviceMetrics.Setup(m => m.IncAccountDeposit());

        // Act
        Deposit.Response response = await _accountService.DepositAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<Deposit.Response.Success>()
            .Which.Account.Should()
            .BeEquivalentTo(accountDto);
    }

    [Fact]
    public async Task DepositAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        var accountId = new AccountId(_faker.Random.Long(0));
        var amount = new Money(_faker.Finance.Amount());

        Deposit.Request request = new(
            user.AuthorizationId,
            accountId.Value,
            amount.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId);

        // Act
        Deposit.Response response = await _accountService.DepositAsync(request, default);

        // Assert
        response.Should().BeOfType<Deposit.Response.Unauthorized>();
    }

    [Fact]
    public async Task DepositAsync_ShouldReturnAccountNotFound_WhenAccountNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        var accountId = new AccountId(_faker.Random.Long(0));
        var amount = new Money(_faker.Finance.Amount());

        Deposit.Request request = new(
            user.AuthorizationId,
            accountId.Value,
            amount.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(accountId);

        // Act
        Deposit.Response response = await _accountService.DepositAsync(request, default);

        // Assert
        response.Should().BeOfType<Deposit.Response.AccountNotFound>();
    }

    [Fact]
    public async Task DepositAsync_ShouldReturnForbidden_WhenAccountNotUsers()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account account = new AutoFaker<AccountTestModel>().Generate().MapToDomain();
        var amount = new Money(_faker.Finance.Amount());

        Deposit.Request request = new(
            user.AuthorizationId,
            account.Id.Value,
            amount.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(account.Id, account);

        // Act
        Deposit.Response response = await _accountService.DepositAsync(request, default);

        // Assert
        response.Should().BeOfType<Deposit.Response.Forbidden>();
    }

    [Fact]
    public async Task DepositAsync_ShouldThrowException_WhenRequestIncorrect()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account account = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();

        Deposit.Request request = new(
            user.AuthorizationId,
            account.Id.Value,
            _faker.Random.Decimal(-10, -1));

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(account.Id, account);

        // Act
        Func<Task> result = async () => await _accountService.DepositAsync(request, default);

        // Assert
        await result.Should().ThrowAsync<ArgumentException>();
    }
}