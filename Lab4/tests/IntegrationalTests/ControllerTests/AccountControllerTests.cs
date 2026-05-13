using AutoBogus;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Application.Abstractions.Persistence.Results;
using BankSystemApi.Application.Options;
using BankSystemApi.Domain.Users;
using BankSystemApi.Domain.ValueObjects;
using BankSystemApi.Grpc;
using Bogus;
using FluentAssertions;
using FluentAssertions.Specialized;
using Grpc.Core;
using IntegrationalTests.Fixtures;
using IntegrationalTests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Account = BankSystemApi.Domain.Accounts.Account;
using User = BankSystemApi.Domain.Users.User;

namespace IntegrationalTests.ControllerTests;

[Collection(nameof(WebApplicationCollectionFixture))]
public sealed class AccountControllerTests : IAsyncDisposable
{
    private readonly AsyncServiceScope _scope;
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IOptionsMonitor<AccountOptions> _accountOptionsMonitor;
    private readonly AccountService.AccountServiceClient _accountServiceClient;

    private readonly Faker _faker = new();

    public AccountControllerTests(WebApplicationFixture fixture)
    {
        _scope = fixture.Services.CreateAsyncScope();
        _userRepository = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _accountRepository = _scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        _accountOptionsMonitor = _scope.ServiceProvider.GetRequiredService<IOptionsMonitor<AccountOptions>>();
        _accountServiceClient = new AccountService.AccountServiceClient(fixture.CreateChannel());
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task CreateAccountAsync_ShouldCreateAccount()
    {
        // Arrange
        User callerUser = await SeedUser();
        User targetUser = await SeedUser();
        var accountNumber = new AccountNumber(_faker.Finance.Account());
        var password = new Password(_faker.Internet.Password());

        var request = new ProtoCreateAccountRequest(
            callerUser.AuthorizationId.ToString(),
            targetUser.Id.Value,
            accountNumber.Value,
            password.Value);

        // Act
        ProtoCreateAccountResponse response = await _accountServiceClient.CreateAsync(request, default);

        // Assert
        response.Account.UserId.Should().Be(targetUser.Id.Value);
    }

    [Fact]
    public async Task CreateAccountAsync_ShouldThrowException_WhenAccountLimitExceeded()
    {
        // Arrange
        User callerUser = await SeedUser();
        User targetUser = await SeedUser();
        var accountNumber = new AccountNumber(_faker.Finance.Account());
        var password = new Password(_faker.Internet.Password());

        long accountLimit = _accountOptionsMonitor.CurrentValue.MaxAmount;
        for (int i = 0; i < accountLimit; i++)
        {
            await SeedAccount(targetUser.Id);
        }

        var request = new ProtoCreateAccountRequest(
            callerUser.AuthorizationId.ToString(),
            targetUser.Id.Value,
            accountNumber.Value,
            password.Value);

        // Act
        Func<Task<ProtoCreateAccountResponse>> responseFunc = async () =>
            await _accountServiceClient.CreateAsync(request, default);

        // Assert
        ExceptionAssertions<RpcException> response = await responseFunc.Should().ThrowAsync<RpcException>();
        response.Which.StatusCode.Should().Be(StatusCode.OutOfRange);
    }

    [Fact]
    public async Task CreateAccountAsync_ShouldThrowException_WhenAccountAlreadyExist()
    {
        // Arrange
        User callerUser = await SeedUser();
        User targetUser = await SeedUser();
        Account account = await SeedAccount(callerUser.Id);

        var request = new ProtoCreateAccountRequest(
            callerUser.AuthorizationId.ToString(),
            targetUser.Id.Value,
            account.Number.Value,
            account.Password.Value);

        // Act
        Func<Task<ProtoCreateAccountResponse>> responseFunc = async () =>
            await _accountServiceClient.CreateAsync(request, default);

        // Assert
        ExceptionAssertions<RpcException> response = await responseFunc.Should().ThrowAsync<RpcException>();
        response.Which.StatusCode.Should().Be(StatusCode.AlreadyExists);
    }

    [Fact]
    public async Task DepositAsync_ShouldDeposit()
    {
        // Arrange
        User user = await SeedUser();
        Account account = await SeedAccount(user.Id);
        var amount = new Money(_faker.Finance.Amount());

        var request = new ProtoDepositRequest(
            user.AuthorizationId.ToString(),
            account.Id.Value.ToString(),
            new Google.Type.Money { DecimalValue = amount.Value });

        // Act
        ProtoDepositResponse response = await _accountServiceClient.DepositAsync(request, default);

        // Assert
        response.Account.Balance.DecimalValue.Should().Be(amount.Value + account.Balance.Value);
    }

    [Fact]
    public async Task DepositAsync_ShouldThrowException_WhenAccountNotUsers()
    {
        // Arrange
        User user = await SeedUser();
        Account account = await SeedAccount(new UserId(_faker.Random.Long()));
        var amount = new Money(_faker.Finance.Amount());

        var request = new ProtoDepositRequest(
            user.AuthorizationId.ToString(),
            account.Id.Value.ToString(),
            new Google.Type.Money { DecimalValue = amount.Value });

        // Act
        Func<Task<ProtoDepositResponse>> responseFunc = async () =>
            await _accountServiceClient.DepositAsync(request, default);

        // Assert
        ExceptionAssertions<RpcException> response = await responseFunc.Should().ThrowAsync<RpcException>();
        response.Which.StatusCode.Should().Be(StatusCode.PermissionDenied);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldWithdraw()
    {
        // Arrange
        User user = await SeedUser();
        Account account = await SeedAccount(user.Id);
        var amount = new Money(_faker.Finance.Amount(max: account.Balance.Value));

        var request = new ProtoWithdrawRequest(
            user.AuthorizationId.ToString(),
            account.Id.Value.ToString(),
            new Google.Type.Money { DecimalValue = amount.Value });

        // Act
        ProtoWithdrawResponse response = await _accountServiceClient.WithdrawAsync(request, default);

        // Assert
        response.Account.Balance.DecimalValue.Should().Be(account.Balance.Value - amount.Value);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldThrowException_WhenAccountNotUsers()
    {
        // Arrange
        User user = await SeedUser();
        Account account = await SeedAccount(new UserId(_faker.Random.Long()));
        var amount = new Money(_faker.Finance.Amount(max: account.Balance.Value));

        var request = new ProtoWithdrawRequest(
            user.AuthorizationId.ToString(),
            account.Id.Value.ToString(),
            new Google.Type.Money { DecimalValue = amount.Value });

        // Act
        Func<Task<ProtoWithdrawResponse>> responseFunc = async () =>
            await _accountServiceClient.WithdrawAsync(request, default);

        // Assert
        ExceptionAssertions<RpcException> response = await responseFunc.Should().ThrowAsync<RpcException>();
        response.Which.StatusCode.Should().Be(StatusCode.PermissionDenied);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldThrowException_WhenInsufficientFunds()
    {
        // Arrange
        User user = await SeedUser();
        Account account = await SeedAccount(user.Id);
        var amount = new Money(_faker.Finance.Amount(min: account.Balance.Value));

        var request = new ProtoWithdrawRequest(
            user.AuthorizationId.ToString(),
            account.Id.Value.ToString(),
            new Google.Type.Money { DecimalValue = amount.Value });

        // Act
        Func<Task<ProtoWithdrawResponse>> responseFunc = async () =>
            await _accountServiceClient.WithdrawAsync(request, default);

        // Assert
        ExceptionAssertions<RpcException> response = await responseFunc.Should().ThrowAsync<RpcException>();
        response.Which.StatusCode.Should().Be(StatusCode.FailedPrecondition);
    }

    private async Task<User> SeedUser()
    {
        User user = new AutoFaker<User>().Generate();
        var result = (AddUserResult.Success)await _userRepository.TryAddAsync([user], default);

        return result.User;
    }

    private async Task<Account> SeedAccount(UserId userId)
    {
        Account account = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, userId)
            .RuleFor(a => a.Number, new AccountNumber(Guid.NewGuid().ToString()))
            .Generate()
            .MapToDomain();
        await _accountRepository.AddAsync([account], default);

        return account;
    }
}