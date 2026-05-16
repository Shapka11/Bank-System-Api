using BankSystemApi.Application.Abstractions.Metrics;
using BankSystemApi.Application.Options;
using BankSystemApi.Application.Providers;
using BankSystemApi.Application.Services;
using Bogus;
using Itmo.Dev.Platform.Common.DateTime;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using UnitTests.MockExtensions;
using UnitTests.Mocks;

namespace UnitTests.ServiceTests.Accounts;

public sealed partial class AccountServiceTests : IAsyncLifetime
{
    private readonly MockPersistenceContext _persistenceContext = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProvider = new(MockBehavior.Strict);
    private readonly Mock<IGuidProvider> _guidProvider = new(MockBehavior.Strict);
    private readonly Mock<IPersistenceTransactionProvider> _transactionProvider = new(MockBehavior.Strict);
    private readonly Mock<IOptionsMonitor<AccountOptions>> _accountOptions = new(MockBehavior.Strict);
    private readonly Mock<IServiceMetrics> _serviceMetrics = new(MockBehavior.Strict);

    private readonly AccountService _accountService;

    private readonly Faker _faker = new()
    {
        Random = new Randomizer(42),
    };

    public AccountServiceTests()
    {
        _transactionProvider.SetupDefaultTransaction();

        _accountService = new AccountService(
            _persistenceContext,
            _dateTimeProvider.Object,
            _guidProvider.Object,
            _transactionProvider.Object,
            _accountOptions.Object,
            NullLogger<AccountService>.Instance,
            _serviceMetrics.Object);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        _persistenceContext.VerifyAll();
        _serviceMetrics.VerifyAll();
        _dateTimeProvider.VerifyAll();
        _guidProvider.VerifyAll();
        _accountOptions.VerifyAll();

        return Task.CompletedTask;
    }
}