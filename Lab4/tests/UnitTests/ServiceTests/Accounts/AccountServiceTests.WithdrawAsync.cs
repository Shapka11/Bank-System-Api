using AutoBogus;
using BankSystemApi.Application.Contracts.Accounts.Models;
using BankSystemApi.Application.Contracts.Accounts.Operations;
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
    public async Task WithdrawAsync_ShouldReturnDto_WhenAccountExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        var amount = new Money(_faker.Finance.Amount());
        var remainder = new Money(_faker.Finance.Amount());
        Account account = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .RuleFor(a => a.Balance, amount + remainder)
            .Generate()
            .MapToDomain();

        Withdraw.Request request = new(
            user.AuthorizationId,
            account.Id.Value,
            amount.Value);

        DateTimeOffset updatedAccountTime = _faker.Date.RecentOffset();
        _dateTimeProvider.Setup(time => time.Current).Returns(updatedAccountTime);

        var updatedAccount = new Account(
            account.Id,
            account.UserId,
            account.Number,
            account.Password,
            remainder,
            account.CreatedAt,
            updatedAccountTime);

        var historyOperation = new WithdrawHistoryOperation(
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
            updatedAccount.Number.Value,
            updatedAccount.Password.Value,
            updatedAccount.Balance.Value,
            updatedAccount.CreatedAt,
            updatedAccount.UpdatedAt);

        _serviceMetrics.Setup(m => m.IncAccountWithdrawal());

        // Act
        Withdraw.Response response = await _accountService.WithdrawAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<Withdraw.Response.Success>()
            .Which.Account.Should()
            .BeEquivalentTo(accountDto);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldReturnUnauthorized_WhenUSerNotFound()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        var accountId = new AccountId(_faker.Random.Guid());
        var amount = new Money(_faker.Finance.Amount());

        Withdraw.Request request = new(
            user.AuthorizationId,
            accountId.Value,
            amount.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId);

        // Act
        Withdraw.Response response = await _accountService.WithdrawAsync(request, default);

        // Assert
        response.Should().BeOfType<Withdraw.Response.Unauthorized>();
    }

    [Fact]
    public async Task WithdrawAsync_ShouldReturnAccountNotFound_WhenAccountNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        var accountId = new AccountId(_faker.Random.Guid());
        var amount = new Money(_faker.Finance.Amount());

        Withdraw.Request request = new(
            user.AuthorizationId,
            accountId.Value,
            amount.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(accountId);

        // Act
        Withdraw.Response response = await _accountService.WithdrawAsync(request, default);

        // Assert
        response.Should().BeOfType<Withdraw.Response.AccountNotFound>();
    }

    [Fact]
    public async Task WithdrawAsync_ShouldReturnForbidden_WhenAccountNotUsers()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        var amount = new Money(_faker.Finance.Amount());
        var remainder = new Money(_faker.Finance.Amount());
        Account account = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.Balance, amount + remainder)
            .Generate()
            .MapToDomain();

        Withdraw.Request request = new(
            user.AuthorizationId,
            account.Id.Value,
            amount.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(account.Id, account);

        // Act
        Withdraw.Response response = await _accountService.WithdrawAsync(request, default);

        // Assert
        response.Should().BeOfType<Withdraw.Response.Forbidden>();
    }

    [Fact]
    public async Task WithdrawAsync_ShouldReturnInsufficientFunds_WhenInsufficientFunds()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        var amount = new Money(_faker.Finance.Amount());
        Account account = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .RuleFor(a => a.Balance, new Money(_faker.Finance.Amount(max: amount.Value)))
            .Generate()
            .MapToDomain();

        Withdraw.Request request = new(
            user.AuthorizationId,
            account.Id.Value,
            amount.Value);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(account.Id, account);

        // Act
        Withdraw.Response response = await _accountService.WithdrawAsync(request, default);

        // Assert
        response.Should().BeOfType<Withdraw.Response.InsufficientFunds>();
    }

    [Fact]
    public async Task WithdrawAsync_ShouldThrowException_WhenRequestIncorrect()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        var amount = new Money(_faker.Finance.Amount());
        Account account = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .RuleFor(a => a.Balance, new Money(_faker.Finance.Amount(min: amount.Value)))
            .Generate()
            .MapToDomain();

        Withdraw.Request request = new(
            user.AuthorizationId,
            account.Id.Value,
            _faker.Random.Decimal(max: -1));

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(account.Id, account);

        // Act
        Func<Task> result = async () => await _accountService.WithdrawAsync(request, default);

        // Assert
        await result.Should().ThrowAsync<ArgumentException>();
    }
}