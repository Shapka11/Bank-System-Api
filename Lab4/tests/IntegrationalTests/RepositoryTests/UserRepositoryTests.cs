using AutoBogus;
using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Application.Abstractions.Persistence.Results;
using BankSystemApi.Domain.Users;
using FluentAssertions;
using IntegrationalTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationalTests.RepositoryTests;

[Collection(nameof(WebApplicationCollectionFixture))]
public sealed class UserRepositoryTests : IAsyncDisposable
{
    private readonly AsyncServiceScope _scope;
    private readonly IUserRepository _userRepository;

    public UserRepositoryTests(WebApplicationFixture fixture)
    {
        _scope = fixture.Services.CreateAsyncScope();
        _userRepository = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task TryAddAsync_ShouldAddUserIdempotent()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();

        // Act
        AddUserResult resultFirstAdd = await _userRepository.TryAddAsync([user], default);
        AddUserResult resultSecondAdd = await _userRepository.TryAddAsync([user], default);

        // Assert
        resultFirstAdd.Should().BeOfType<AddUserResult.Success>().Which.User.Id.Value.Should().Be(1);
        resultSecondAdd.Should().BeOfType<AddUserResult.AlreadyExist>();
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnUser_WhenQueryByAuthId()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        const int pageSize = 1;
        var query = UserQuery.Build(builder => builder
            .WithAuthorizationId(user.AuthorizationId)
            .WithPageSize(pageSize));

        // Act
        var addResult = await _userRepository.TryAddAsync([user], default) as AddUserResult.Success;
        List<User> resultUsers = await _userRepository.QueryAsync(query, default).ToListAsync();

        // Assert
        resultUsers.Should().HaveCount(pageSize);
        addResult.Should().NotBeNull();
        resultUsers.Should().Contain(addResult.User);
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnUser_WhenQueryById()
    {
        // Arrange
        User user = new AutoFaker<User>().Generate();
        var addResult = (AddUserResult.Success)await _userRepository.TryAddAsync([user], default);
        User addedUser = addResult.User;

        const int pageSize = 1;
        var query = UserQuery.Build(builder => builder
            .WithId(addedUser.Id)
            .WithPageSize(pageSize));

        // Act
        List<User> resultUsers = await _userRepository.QueryAsync(query, default).ToListAsync();

        // Assert
        resultUsers.Should().HaveCount(pageSize);
        resultUsers.Should().Contain(addedUser);
    }
}