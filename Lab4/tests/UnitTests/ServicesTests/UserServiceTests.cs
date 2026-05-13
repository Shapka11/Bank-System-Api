using AutoBogus;
using BankSystemApi.Application.Abstractions.Persistence.Results;
using BankSystemApi.Application.Contracts.Users.Operations;
using BankSystemApi.Application.Services;
using BankSystemApi.Domain.Users;
using Bogus;
using FluentAssertions;
using Itmo.Dev.Platform.Common.DateTime;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UnitTests.MockExtensions;
using UnitTests.Mocks;

namespace UnitTests.ServicesTests;

public sealed class UserServiceTests : IAsyncLifetime
{
    private readonly MockPersistenceContext _persistenceContext = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProvider = new(MockBehavior.Strict);

    private readonly UserService _userService;

    private readonly Faker _faker = new();

    public UserServiceTests()
    {
        _userService = new UserService(
            _persistenceContext,
            _dateTimeProvider.Object,
            NullLogger<UserService>.Instance);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        _persistenceContext.VerifyAll();
        _dateTimeProvider.VerifyAll();

        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateAsync_ShouldSuccess_WhenUserAlreadyExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        AddUser.Request request = new(user.AuthorizationId);

        _dateTimeProvider.Setup(time => time.Current).Returns(_faker.Date.RecentOffset());

        var addUserResult = new AddUserResult.AlreadyExist();
        _persistenceContext.UsersRepository
            .SetupTryAddUser(user, addUserResult)
            .SetupQueryUserByAuthId(user.AuthorizationId, user);

        // Act
        AddUser.Response response = await _userService.AddAsync(request, default);

        // Assert
        response.Should().BeOfType<AddUser.Response.Success>();
    }

    [Fact]
    public async Task CreateAsync_ShouldSuccess_WhenUserNotExist()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        AddUser.Request request = new(user.AuthorizationId);

        _dateTimeProvider.Setup(time => time.Current).Returns(_faker.Date.RecentOffset());

        var addUserResult = new AddUserResult.Success(new User(user.Id, user.AuthorizationId, user.CreatedAt));
        _persistenceContext.UsersRepository
            .SetupTryAddUser(user, addUserResult);

        // Act
        AddUser.Response response = await _userService.AddAsync(request, default);

        // Assert
        response.Should().BeOfType<AddUser.Response.Success>();
    }
}