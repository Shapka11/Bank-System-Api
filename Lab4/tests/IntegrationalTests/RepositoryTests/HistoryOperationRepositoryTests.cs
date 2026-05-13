using AutoBogus;
using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using BankSystemApi.Domain.HistoryOperations.Invoices;
using FluentAssertions;
using IntegrationalTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationalTests.RepositoryTests;

[Collection(nameof(WebApplicationCollectionFixture))]
public sealed class HistoryOperationRepositoryTests : IAsyncDisposable
{
    private readonly AsyncServiceScope _scope;
    private readonly IHistoryOperationRepository _historyOperationRepository;

    public HistoryOperationRepositoryTests(WebApplicationFixture fixture)
    {
        _scope = fixture.Services.CreateAsyncScope();
        _historyOperationRepository = _scope.ServiceProvider.GetRequiredService<IHistoryOperationRepository>();
    }

    public static IEnumerable<object[]> GetHistoryOperations()
    {
        yield return new object[] { new AutoFaker<CreateAccountHistoryOperation>().Generate() };
        yield return new object[] { new AutoFaker<DepositHistoryOperation>().Generate() };
        yield return new object[] { new AutoFaker<WithdrawHistoryOperation>().Generate() };
        yield return new object[] { new AutoFaker<CheckBalanceHistoryOperation>().Generate() };
        yield return new object[] { new AutoFaker<InvoiceIssuedHistoryOperation>().Generate() };
        yield return new object[] { new AutoFaker<InvoicePaymentSentHistoryOperation>().Generate() };
        yield return new object[] { new AutoFaker<InvoicePaymentReceivedHistoryOperation>().Generate() };
        yield return new object[] { new AutoFaker<InvoiceReceivedHistoryOperation>().Generate() };
        yield return new object[] { new AutoFaker<InvoiceRevokedHistoryOperation>().Generate() };
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Theory]
    [MemberData(nameof(GetHistoryOperations))]
    public async Task AddAsync_ShouldAddOperation(HistoryOperation operation)
    {
        // Act
        HistoryOperation dbOperations = await _historyOperationRepository.AddAsync([operation], default).FirstAsync();

        // Assert
        dbOperations.AccountId.Should().Be(operation.AccountId);
        dbOperations.Should().BeOfType(operation.GetType());
    }

    [Theory]
    [MemberData(nameof(GetHistoryOperations))]
    public async Task QueryAsync_ShouldReturnOperation(HistoryOperation operation)
    {
        // Arrange
        await _historyOperationRepository.AddAsync([operation], default).FirstAsync();
        var query = HistoryOperationQuery.Build(builder => builder
            .WithAccountId(operation.AccountId)
            .WithPageSize(1));

        // Act
        HistoryOperation dbOperations = await _historyOperationRepository.QueryAsync(query, default).FirstAsync();

        // Assert
        dbOperations.AccountId.Should().Be(operation.AccountId);
        dbOperations.Should().BeOfType(operation.GetType());
    }
}