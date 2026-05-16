using AutoBogus;
using BankSystemApi.Application.Contracts.Accounts.Operations;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.Users;
using FluentAssertions;
using UnitTests.MockExtensions;

namespace UnitTests.ServiceTests.Accounts;

public sealed partial class AccountServiceTests
{
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