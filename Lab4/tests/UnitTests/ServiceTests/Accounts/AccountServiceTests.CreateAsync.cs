using AutoBogus;
using BankSystemApi.Application.Contracts.Accounts.Models;
using BankSystemApi.Application.Contracts.Accounts.Operations;
using BankSystemApi.Application.Options;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using UnitTests.MockExtensions;

namespace UnitTests.ServiceTests.Accounts;

public sealed partial class AccountServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldReturnDto_WhenAccountNotExist()
    {
        // Arrange
        Faker localGuidFaker = new()
        {
            Random = new Randomizer(42),
        };
        Guid accountGuid = localGuidFaker.Random.Guid();
        var createdAccountId = new AccountId(accountGuid);
        _guidProvider.Setup(gp => gp.NewGuid()).Returns(accountGuid);

        const int targetUserAmountAccounts = 4;
        var option = new AccountOptions { MaxAmount = targetUserAmountAccounts + 1 };
        _accountOptions.Setup(op => op.CurrentValue).Returns(option);

        User callerUser = new AutoFaker<User>().Generate();
        User targetUser = new AutoFaker<User>().Generate();
        List<Account> targetUserAccounts = new AutoFaker<Account>().Generate(targetUserAmountAccounts);

        CreateAccount.Request request = new(
            callerUser.AuthorizationId,
            targetUser.Id.Value,
            _faker.Finance.Account(),
            _faker.Internet.Password());

        DateTimeOffset currentTime = localGuidFaker.Date.RecentOffset();
        _dateTimeProvider.Setup(time => time.Current).Returns(currentTime);

        var createdAccountPassword = new Password(request.Password);
        var createdAccountNumbers = new AccountNumber(request.AccountNumber);
        var createdAccount = new Account(
            createdAccountId,
            targetUser.Id,
            createdAccountNumbers,
            createdAccountPassword,
            Money.Zero,
            currentTime,
            currentTime);

        var historyOperation = new CreateAccountHistoryOperation(
            HistoryOperationId.Default,
            createdAccountId,
            currentTime);
        var expectedHistoryOperationId = new HistoryOperationId(1);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(callerUser.AuthorizationId, callerUser)
            .SetupQueryUserById(targetUser.Id, targetUser);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountByUserId(targetUser.Id, targetUserAccounts.ToArray())
            .SetupQueryAccountByNumber(createdAccountNumbers)
            .SetupAddAccount(createdAccount);

        _persistenceContext.HistoryOperationsRepository
            .SetupAddHistoryOperation([historyOperation], [expectedHistoryOperationId]);

        var accountDto = new AccountDto(
            createdAccount.Id.Value,
            createdAccount.UserId.Value,
            createdAccount.Number.Value,
            createdAccount.Password.Value,
            createdAccount.Balance.Value,
            createdAccount.CreatedAt,
            createdAccount.UpdatedAt);

        _serviceMetrics.Setup(m => m.IncAccountCreated());

        // Act
        CreateAccount.Response response = await _accountService.CreateAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<CreateAccount.Response.Success>()
            .Which.Account.Should()
            .BeEquivalentTo(accountDto);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnUnauthorized_WhenCallerUserNotExist()
    {
        // Arrange
        User callerUser = new AutoFaker<User>().Generate();
        User targetUser = new AutoFaker<User>().Generate();

        CreateAccount.Request request = new(
            callerUser.AuthorizationId,
            targetUser.Id.Value,
            _faker.Finance.Account(),
            _faker.Internet.Password());

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(callerUser.AuthorizationId);

        // Act
        CreateAccount.Response response = await _accountService.CreateAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<CreateAccount.Response.Unauthorized>()
            .Which.UserId.Should()
            .BeEquivalentTo(request.CallerUserId.ToString());
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnUnauthorized_WhenTargetUserNotExist()
    {
        // Arrange
        User callerUser = new AutoFaker<User>().Generate();
        User targetUser = new AutoFaker<User>().Generate();

        CreateAccount.Request request = new(
            callerUser.AuthorizationId,
            targetUser.Id.Value,
            _faker.Finance.Account(),
            _faker.Internet.Password());

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(callerUser.AuthorizationId, callerUser)
            .SetupQueryUserById(targetUser.Id);

        // Act
        CreateAccount.Response response = await _accountService.CreateAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<CreateAccount.Response.Unauthorized>()
            .Which.UserId.Should()
            .BeEquivalentTo(request.TargetUserId.ToString());
    }

    [Theory]
    [InlineData(6, 6)]
    [InlineData(7, 6)]
    public async Task CreateAsync_ShouldReturnReachedAccountLimit_WhenTargetUserHasLimit(
        int targetUserAmountAccounts,
        int maxAmountAccounts)
    {
        // Arrange
        var option = new AccountOptions { MaxAmount = maxAmountAccounts };
        _accountOptions.Setup(op => op.CurrentValue).Returns(option);

        User callerUser = new AutoFaker<User>().Generate();
        User targetUser = new AutoFaker<User>().Generate();
        List<Account> targetUserAccounts = new AutoFaker<Account>().Generate(targetUserAmountAccounts);

        CreateAccount.Request request = new(
            callerUser.AuthorizationId,
            targetUser.Id.Value,
            _faker.Finance.Account(),
            _faker.Internet.Password());

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(callerUser.AuthorizationId, callerUser)
            .SetupQueryUserById(targetUser.Id, targetUser);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountByUserId(targetUser.Id, targetUserAccounts.ToArray());

        // Act
        CreateAccount.Response response = await _accountService.CreateAsync(request, default);

        // Assert
        response.Should().BeOfType<CreateAccount.Response.ReachedAccountLimit>();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnAccountAlreadyExists_WhenAccountExist()
    {
        // Arrange
        const int targetUserAmountAccounts = 6;
        var option = new AccountOptions { MaxAmount = targetUserAmountAccounts + 1 };
        _accountOptions.Setup(op => op.CurrentValue).Returns(option);

        User callerUser = new AutoFaker<User>().Generate();
        User targetUser = new AutoFaker<User>().Generate();
        List<Account> targetUserAccounts = new AutoFaker<Account>().Generate(targetUserAmountAccounts);

        CreateAccount.Request request = new(
            callerUser.AuthorizationId,
            targetUser.Id.Value,
            _faker.Finance.Account(),
            _faker.Internet.Password());
        var createdAccountNumber = new AccountNumber(request.AccountNumber);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(callerUser.AuthorizationId, callerUser)
            .SetupQueryUserById(targetUser.Id, targetUser);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountByUserId(targetUser.Id, targetUserAccounts.ToArray())
            .SetupQueryAccountByNumber(createdAccountNumber, new AutoFaker<Account>().Generate());

        // Act
        CreateAccount.Response response = await _accountService.CreateAsync(request, default);

        // Assert
        response.Should().BeOfType<CreateAccount.Response.AccountAlreadyExists>();
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("some-number", " ")]
    [InlineData("", "some-password")]
    public async Task CreateAsync_ShouldThrowException_WhenRequestIncorrect(
        string accountNumber,
        string password)
    {
        User callerUser = new AutoFaker<User>().Generate();
        User targetUser = new AutoFaker<User>().Generate();

        CreateAccount.Request request = new(
            callerUser.AuthorizationId,
            targetUser.Id.Value,
            accountNumber,
            password);

        // Act
        Func<Task> result = async () => await _accountService.CreateAsync(request, default);

        // Assert
        await result.Should().ThrowAsync<Exception>();
    }
}