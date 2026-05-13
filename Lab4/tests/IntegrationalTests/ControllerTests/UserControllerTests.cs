using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Application.Specifications;
using BankSystemApi.Domain.Users;
using BankSystemApi.Grpc;
using Bogus;
using FluentAssertions;
using FluentAssertions.Specialized;
using IntegrationalTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using User = BankSystemApi.Domain.Users.User;

namespace IntegrationalTests.ControllerTests;

[Collection(nameof(WebApplicationCollectionFixture))]
public sealed class UserControllerTests : IAsyncDisposable
{
    private readonly AsyncServiceScope _scope;
    private readonly IUserRepository _userRepository;
    private readonly UserService.UserServiceClient _userServiceClient;

    private readonly Faker _faker = new();

    public UserControllerTests(WebApplicationFixture fixture)
    {
        _scope = fixture.Services.CreateAsyncScope();
        _userRepository = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _userServiceClient = new UserService.UserServiceClient(fixture.CreateChannel());
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task AddUserAsync_ShouldAddUser()
    {
        // Arrange
        Guid authId = _faker.Random.Guid();
        var request = new AddUserRequest(authId.ToString());

        // Act
        Func<Task<AddUserResponse>> responseFunc = async () => await _userServiceClient.AddAsync(
            request,
            cancellationToken: default);

        // Assert
        AndWhichConstraint<GenericAsyncFunctionAssertions<AddUserResponse>, AddUserResponse> response =
            await responseFunc.Should().NotThrowAsync();

        User? user = await _userRepository.FindByIdAsync(new UserId(response.Subject.User.Id), default);
        user.Should().NotBeNull();
        user.AuthorizationId.Should().Be(authId);
    }
}