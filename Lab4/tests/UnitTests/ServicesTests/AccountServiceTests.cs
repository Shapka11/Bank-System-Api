using AutoBogus;
using BankSystemApi.Application.Abstractions.Metrics;
using BankSystemApi.Application.Contracts.Accounts.Models;
using BankSystemApi.Application.Contracts.Accounts.Operations;
using BankSystemApi.Application.Options;
using BankSystemApi.Application.Providers;
using BankSystemApi.Application.Services;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using Bogus;
using FluentAssertions;
using Itmo.Dev.Platform.Common.DateTime;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using UnitTests.MockExtensions;
using UnitTests.Mocks;
using UnitTests.Models;

namespace UnitTests.ServicesTests;

public sealed class AccountServiceTests : IAsyncLifetime
{
    private readonly MockPersistenceContext _persistenceContext = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProvider = new(MockBehavior.Strict);
    private readonly Mock<IGuidProvider> _guidProvider = new(MockBehavior.Strict);
    private readonly Mock<IPersistenceTransactionProvider> _transactionProvider = new(MockBehavior.Strict);
    private readonly Mock<IOptionsMonitor<AccountOptions>> _accountOptions = new(MockBehavior.Strict);
    private readonly Mock<IServiceMetrics> _serviceMetrics = new(MockBehavior.Strict);

    private readonly AccountService _accountService;

    private readonly Faker _faker = new();

    public AccountServiceTests()
    {
        _transactionProvider.SetupDefaultTransaction();

        _accountService = new AccountService(
            _persistenceContext,
            _dateTimeProvider.Object,
            _guidProvider.Object,
            _transactionProvider.Object,
            _accountOptions.Object,
            NullLogger<AccountService>.Instance,
            _serviceMetrics.Object);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        _persistenceContext.VerifyAll();
        _serviceMetrics.VerifyAll();
        _dateTimeProvider.VerifyAll();
        _guidProvider.VerifyAll();
        _accountOptions.VerifyAll();

        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnDto_WhenAccountNotExist()
    {
        // Arrange
        Faker localFaker = new();
        Guid accountGuid = localFaker.Random.Guid();
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

        DateTimeOffset currentTime = localFaker.Date.RecentOffset();
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
        var accountId = new AccountId(_faker.Random.Guid());
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
        var accountId = new AccountId(_faker.Random.Guid());
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

    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(2, 1, false)]
    [InlineData(1, 2, true)]
    public async Task GetAsync_ShouldReturnDto_WhenAccountExist(
        int pageSize,
        int accountCount,
        bool pageTokenReturned)
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        List<Account> accounts = new AutoFaker<Account>().Generate(accountCount);

        GetAccounts.Request request = new(user.AuthorizationId, pageSize, null);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountByUserId(user.Id, accounts.ToArray());

        // Act
        GetAccounts.Response response = await _accountService.GetAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<GetAccounts.Response.Success>()
            .Which.Accounts.Should()
            .HaveCount(accountCount);

        if (pageTokenReturned)
        {
            response
                .Should()
                .BeOfType<GetAccounts.Response.Success>()
                .Which.PageToken.Should()
                .NotBeNull();
        }
        else
        {
            response
                .Should()
                .BeOfType<GetAccounts.Response.Success>()
                .Which.PageToken.Should()
                .BeNull();
        }
    }

    [Fact]
    public async Task GetAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        GetAccounts.Request request = new(user.AuthorizationId, _faker.Random.Int(0), null);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId);

        // Act
        GetAccounts.Response response = await _accountService.GetAsync(request, default);

        // Assert
        response.Should().BeOfType<GetAccounts.Response.Unauthorized>();
    }
}