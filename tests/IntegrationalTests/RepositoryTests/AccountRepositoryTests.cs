using AutoBogus;
using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Application.Specifications;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.ValueObjects;
using FluentAssertions;
using IntegrationalTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationalTests.RepositoryTests;

[Collection(nameof(WebApplicationCollectionFixture))]
public sealed class AccountRepositoryTests : BaseRepositoryTests
{
    private readonly IAccountRepository _accountRepository;

    public AccountRepositoryTests(WebApplicationFixture fixture) : base(fixture)
    {
        _accountRepository = Scope.ServiceProvider.GetRequiredService<IAccountRepository>();
    }

    [Fact]
    public async Task AddAsync_ShouldAddAccount()
    {
        // Arrange
        Account account = new AutoFaker<Account>().Generate();

        // Act
        Account dbAccount = await _accountRepository.AddAsync([account], default).FirstAsync();

        // Assert
        dbAccount.Number.Should().Be(account.Number);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAccount()
    {
        // Arrange
        Account account = new AutoFaker<Account>().RuleFor(a => a.Balance, new Money(1)).Generate();

        account = await _accountRepository.AddAsync([account], default).FirstAsync();
        account.Deposit(new Money(1));

        // Act
        await _accountRepository.UpdateAsync([account], default);
        Account? dbAccount = await _accountRepository.FindAccountByNumberAsync(account.Number, default);

        // Assert
        dbAccount.Should().NotBeNull();
        dbAccount.Balance.Value.Should().Be(2);
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnAccount_WhenQueryByNumber()
    {
        // Arrange
        Account account = await SeedAccountAsync();

        // Act
        Account? dbAccount = await _accountRepository.FindAccountByNumberAsync(account.Number, default);

        // Assert
        dbAccount.Should().NotBeNull();
        dbAccount.Number.Should().Be(account.Number);
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnAccount_WhenQueryById()
    {
        // Arrange
        Account account = await SeedAccountAsync();

        // Act
        Account? dbAccount = await _accountRepository.FindAccountByIdAsync(account.Id, default);

        // Assert
        dbAccount.Should().NotBeNull();
        dbAccount.Id.Should().Be(account.Id);
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnAccount_WhenQueryByUserId()
    {
        // Arrange
        Account account = await SeedAccountAsync();

        // Act
        Account[] dbAccounts = await _accountRepository.GetAllByUserId(account.UserId, default);

        // Assert
        dbAccounts.Should().HaveCount(1);
        dbAccounts.First().Number.Should().Be(account.Number);
    }

    private async Task<Account> SeedAccountAsync()
    {
        Account account = new AutoFaker<Account>().Generate();
        account = await _accountRepository.AddAsync([account], default).FirstAsync();

        var query = AccountQuery.Build(builder => builder
            .WithAccountNumber(account.Number)
            .WithPageSize(1));

        return await _accountRepository.QueryAsync(query, default).FirstAsync();
    }
}