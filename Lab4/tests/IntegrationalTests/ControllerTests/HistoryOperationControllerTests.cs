using AutoBogus;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Application.Abstractions.Persistence.Results;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations.Accounts;
using BankSystemApi.Domain.HistoryOperations.Invoices;
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
using Account = BankSystemApi.Domain.Accounts.Account;
using HistoryOperation = BankSystemApi.Domain.HistoryOperations.HistoryOperation;
using OperationDataOneofCase = BankSystemApi.Grpc.HistoryOperation.OperationDataOneofCase;
using User = BankSystemApi.Domain.Users.User;

namespace IntegrationalTests.ControllerTests;

[Collection(nameof(WebApplicationCollectionFixture))]
public sealed class HistoryOperationControllerTests : IAsyncDisposable
{
    private readonly AsyncServiceScope _scope;
    private readonly IHistoryOperationRepository _historyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly HistoryOperationService.HistoryOperationServiceClient _historyServiceClient;

    private readonly Faker _faker = new();

    public HistoryOperationControllerTests(WebApplicationFixture fixture)
    {
        _scope = fixture.Services.CreateAsyncScope();
        _historyRepository = _scope.ServiceProvider.GetRequiredService<IHistoryOperationRepository>();
        _userRepository = _scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _accountRepository = _scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        _historyServiceClient = new HistoryOperationService.HistoryOperationServiceClient(fixture.CreateChannel());
    }

    public static IEnumerable<object[]> GetHistoryOperations()
    {
        yield return CreateTestCase<CreateAccountHistoryOperation>(OperationDataOneofCase.CreateAccount);
        yield return CreateTestCase<DepositHistoryOperation>(OperationDataOneofCase.Deposit);
        yield return CreateTestCase<WithdrawHistoryOperation>(OperationDataOneofCase.Withdraw);
        yield return CreateTestCase<CheckBalanceHistoryOperation>(OperationDataOneofCase.CheckBalance);
        yield return CreateTestCase<InvoiceIssuedHistoryOperation>(OperationDataOneofCase.InvoiceIssued);
        yield return CreateTestCase<InvoiceReceivedHistoryOperation>(OperationDataOneofCase.InvoiceReceived);
        yield return CreateTestCase<InvoicePaymentSentHistoryOperation>(OperationDataOneofCase.InvoicePaymentSent);
        yield return CreateTestCase<InvoicePaymentReceivedHistoryOperation>(
            OperationDataOneofCase
                .InvoicePaymentReceived);
        yield return CreateTestCase<InvoiceRevokedHistoryOperation>(OperationDataOneofCase.InvoiceRevoked);
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Theory]
    [MemberData(nameof(GetHistoryOperations))]
    public async Task GetHistoryAsync_ShouldGetHistory(
        Func<AccountId, HistoryOperation> operationFactory,
        OperationDataOneofCase oneOfOperation)
    {
        // Arrange
        User user = await SeedUser();
        Account account = await SeedAccount(user.Id);
        const int pageSize = 1;
        var request = new GetHistoryOperationRequest(
            user.AuthorizationId.ToString(),
            account.Id.Value.ToString(),
            new Pagination { PageSize = pageSize });

        HistoryOperation operation = operationFactory(account.Id);
        await SeedHistoryOperation([operation]);

        // Act
        Func<Task<GetHistoryOperationResponse>> responseFunc = async () => await _historyServiceClient.GetAsync(
            request,
            cancellationToken: default);

        // Assert
        AndWhichConstraint<GenericAsyncFunctionAssertions<GetHistoryOperationResponse>,
            GetHistoryOperationResponse> response =
            await responseFunc.Should().NotThrowAsync();
        response.Subject.History.Count.Should().Be(pageSize);
        response.Subject.History.First().OperationDataCase.Should().Be(oneOfOperation);
        response.Subject.History.First().AccountId.Should().Be(account.Id.Value.ToString());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task GetHistoryAsync_ShouldGetHistoryNoMorePageSizeQuantity(int pageSize)
    {
        // Arrange
        User user = await SeedUser();
        Account account = await SeedAccount(user.Id);
        var request = new GetHistoryOperationRequest(
            user.AuthorizationId.ToString(),
            account.Id.Value.ToString(),
            new Pagination { PageSize = pageSize });

        HistoryOperation[] operations = new AutoFaker<CreateAccountHistoryOperation>()
            .RuleFor(o => o.AccountId, account.Id)
            .Generate(4)
            .ToArray();
        await SeedHistoryOperation(operations);

        // Act
        GetHistoryOperationResponse response = await _historyServiceClient.GetAsync(request, default);

        // Assert
        response.History.Count.Should().BeLessThanOrEqualTo(pageSize);
    }

    [Fact]
    public async Task GetHistoryAsync_ShouldThrowException_WhenAccountNotUsers()
    {
        // Arrange
        User user = await SeedUser();
        Account account = await SeedAccount(new UserId(_faker.Random.Long()));
        const int pageSize = 1;
        var request = new GetHistoryOperationRequest(
            user.AuthorizationId.ToString(),
            account.Id.Value.ToString(),
            new Pagination { PageSize = pageSize });

        // Act
        Func<Task<GetHistoryOperationResponse>> responseFunc = async () => await _historyServiceClient.GetAsync(
            request,
            cancellationToken: default);

        // Assert
        ExceptionAssertions<RpcException> exception = await responseFunc.Should().ThrowAsync<RpcException>();
        exception.Which.StatusCode.Should().Be(StatusCode.PermissionDenied);
    }

    private static object[] CreateTestCase<TOperation>(OperationDataOneofCase operationCase)
        where TOperation : HistoryOperation
    {
        Func<AccountId, HistoryOperation> factory = accountId =>
            new AutoFaker<TOperation>().RuleFor(h => h.AccountId, accountId).Generate();

        return [factory, operationCase];
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

    private async Task SeedHistoryOperation(HistoryOperation[] operations)
    {
        await _historyRepository.AddAsync(operations, default).FirstAsync();
    }
}