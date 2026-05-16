using AutoBogus;
using BankSystemApi.Application.Contracts.HistoryOperations.Operations;
using BankSystemApi.Application.Services;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using BankSystemApi.Domain.Users;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using UnitTests.MockExtensions;
using UnitTests.Mocks;
using UnitTests.Models;

namespace UnitTests.ServiceTests;

public sealed class HistoryOperationServiceTests : IAsyncLifetime
{
    private readonly MockPersistenceContext _persistenceContext = new();

    private readonly HistoryOperationService _historyOperationService;

    private readonly Faker _faker = new()
    {
        Random = new Randomizer(42),
    };

    public HistoryOperationServiceTests()
    {
        _historyOperationService = new HistoryOperationService(
            _persistenceContext,
            NullLogger<HistoryOperationService>.Instance);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        _persistenceContext.VerifyAll();

        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(2, 1, false)]
    [InlineData(1, 2, true)]
    public async Task GetAsync_ShouldReturnDtos(
        int pageSize,
        int operationCount,
        bool pageTokenReturned)
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account account = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();

        List<CreateAccountHistoryOperation> operations = new AutoFaker<CreateAccountHistoryOperation>()
            .Generate(operationCount);

        GetHistoryOperations.Request request = new(
            user.AuthorizationId,
            account.Id.Value,
            pageSize,
            null);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(account.Id, account);

        _persistenceContext.HistoryOperationsRepository
            .SetupQueryHistoryOperationByAccountId(account.Id, operations.ToArray());

        // Act
        GetHistoryOperations.Response response = await _historyOperationService.GetAsync(request, default);

        // Assert
        response
            .Should()
            .BeOfType<GetHistoryOperations.Response.Success>()
            .Which.History.Should()
            .HaveCount(operationCount);

        if (pageTokenReturned)
        {
            response
                .Should()
                .BeOfType<GetHistoryOperations.Response.Success>()
                .Which.PageToken.Should()
                .NotBeNull();
        }
        else
        {
            response
                .Should()
                .BeOfType<GetHistoryOperations.Response.Success>()
                .Which.PageToken.Should()
                .BeNull();
        }
    }

    [Fact]
    public async Task GetAsync_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account account = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();

        GetHistoryOperations.Request request = new(
            user.AuthorizationId,
            account.Id.Value,
            _faker.Random.Int(0),
            null);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId);

        // Act
        GetHistoryOperations.Response response = await _historyOperationService.GetAsync(request, default);

        // Assert
        response.Should().BeOfType<GetHistoryOperations.Response.Unauthorized>();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAccountNotFound_WhenAccountNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account account = new AutoFaker<AccountTestModel>()
            .RuleFor(a => a.UserId, user.Id)
            .Generate()
            .MapToDomain();

        GetHistoryOperations.Request request = new(
            user.AuthorizationId,
            account.Id.Value,
            _faker.Random.Int(0),
            null);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(account.Id);

        // Act
        GetHistoryOperations.Response response = await _historyOperationService.GetAsync(request, default);

        // Assert
        response.Should().BeOfType<GetHistoryOperations.Response.AccountNotFound>();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnForbidden_WhenAccountNotUsers()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        Account account = new AutoFaker<Account>().Generate();

        GetHistoryOperations.Request request = new(
            user.AuthorizationId,
            account.Id.Value,
            _faker.Random.Int(0),
            null);

        _persistenceContext.UsersRepository
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        _persistenceContext.AccountsRepository
            .SetupQueryAccountById(account.Id, account);

        // Act
        GetHistoryOperations.Response response = await _historyOperationService.GetAsync(request, default);

        // Assert
        response.Should().BeOfType<GetHistoryOperations.Response.Forbidden>();
    }
}