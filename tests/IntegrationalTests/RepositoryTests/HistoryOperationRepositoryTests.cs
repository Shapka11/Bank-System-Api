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
public sealed class HistoryOperationRepositoryTests : BaseRepositoryTests
{
    private readonly IHistoryOperationRepository _historyOperationRepository;

    public HistoryOperationRepositoryTests(WebApplicationFixture fixture) : base(fixture)
    {
        _historyOperationRepository = Scope.ServiceProvider.GetRequiredService<IHistoryOperationRepository>();
    }

    public static TheoryData<HistoryOperation> GetHistoryOperations() => new()
    {
        new AutoFaker<CreateAccountHistoryOperation>().Generate(),
        new AutoFaker<DepositHistoryOperation>().Generate(),
        new AutoFaker<WithdrawHistoryOperation>().Generate(),
        new AutoFaker<CheckBalanceHistoryOperation>().Generate(),
        new AutoFaker<InvoiceIssuedHistoryOperation>().Generate(),
        new AutoFaker<InvoicePaymentSentHistoryOperation>().Generate(),
        new AutoFaker<InvoicePaymentReceivedHistoryOperation>().Generate(),
        new AutoFaker<InvoiceReceivedHistoryOperation>().Generate(),
        new AutoFaker<InvoiceRevokedHistoryOperation>().Generate(),
    };

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