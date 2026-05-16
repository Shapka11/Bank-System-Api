using BankSystemApi.Application.Abstractions.Metrics;
using BankSystemApi.Application.Services;
using BankSystemApi.Domain.Invoices.States;
using Bogus;
using Itmo.Dev.Platform.Common.DateTime;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UnitTests.MockExtensions;
using UnitTests.Mocks;

namespace UnitTests.ServiceTests.Invoices;

public sealed partial class InvoiceServiceTests : IAsyncLifetime
{
    private readonly MockPersistenceContext _persistenceContext = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProvider = new(MockBehavior.Strict);
    private readonly Mock<IPersistenceTransactionProvider> _transactionProvider = new(MockBehavior.Strict);
    private readonly Mock<IServiceMetrics> _serviceMetrics = new(MockBehavior.Strict);

    private readonly InvoiceService _invoiceService;

    private readonly Faker _faker = new()
    {
        Random = new Randomizer(42),
    };

    public InvoiceServiceTests()
    {
        _transactionProvider.SetupDefaultTransaction();

        _invoiceService = new InvoiceService(
            _persistenceContext,
            _dateTimeProvider.Object,
            _transactionProvider.Object,
            NullLogger<InvoiceService>.Instance,
            _serviceMetrics.Object);
    }

    public static TheoryData<IInvoiceState> GetTerminalStates() => new()
    {
        new PaidInvoiceState(),
        new RevokedInvoiceState(),
    };

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        _persistenceContext.VerifyAll();
        _dateTimeProvider.VerifyAll();
        _serviceMetrics.VerifyAll();

        return Task.CompletedTask;
    }
}