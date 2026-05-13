using AutoBogus;
using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Application.Specifications;
using BankSystemApi.Domain.Invoices;
using BankSystemApi.Domain.Invoices.States;
using FluentAssertions;
using IntegrationalTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationalTests.RepositoryTests;

[Collection(nameof(WebApplicationCollectionFixture))]
public sealed class InvoiceRepositoryTests : IAsyncDisposable
{
    private readonly AsyncServiceScope _scope;
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceRepositoryTests(WebApplicationFixture fixture)
    {
        _scope = fixture.Services.CreateAsyncScope();
        _invoiceRepository = _scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_ShouldAddInvoice()
    {
        // Arrange
        Invoice invoice = new AutoFaker<Invoice>().RuleFor(i => i.State, new CreatedInvoiceState()).Generate();

        // Act
        Invoice dbInvoice = await _invoiceRepository.AddAsync([invoice], default).FirstAsync();

        // Assert
        dbInvoice.SenderAccountId.Should().Be(invoice.SenderAccountId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateInvoice()
    {
        // Arrange
        Invoice invoice = await SeedInvoiceAsync(new CreatedInvoiceState());
        invoice.Pay();

        // Act
        await _invoiceRepository.UpdateAsync([invoice], default);
        Invoice? updatedInvoice = await _invoiceRepository.FindById(invoice.Id, default);

        // Assert
        updatedInvoice.Should().NotBeNull();
        updatedInvoice.SenderAccountId.Should().Be(invoice.SenderAccountId);
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnInvoice_WhenQueryById()
    {
        // Arrange
        Invoice invoice = await SeedInvoiceAsync(new CreatedInvoiceState());

        // Act
        Invoice? dbInvoice = await _invoiceRepository.FindById(invoice.Id, default);

        // Assert
        dbInvoice.Should().NotBeNull();
        dbInvoice.Should().BeEquivalentTo(invoice);
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnInvoice_WhenQueryBySenderAccountId()
    {
        // Arrange
        Invoice invoice = await SeedInvoiceAsync(new CreatedInvoiceState());
        var query = InvoiceQuery.Build(builder => builder
            .WithSenderAccountId(invoice.SenderAccountId)
            .WithPageSize(1));

        // Act
        Invoice dbInvoice = await _invoiceRepository.QueryAsync(query, default).FirstAsync();

        // Assert
        dbInvoice.Should().BeEquivalentTo(invoice);
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnInvoice_WhenQueryByReceiverAccountId()
    {
        // Arrange
        Invoice invoice = await SeedInvoiceAsync(new CreatedInvoiceState());
        var query = InvoiceQuery.Build(builder => builder
            .WithReceiverAccountId(invoice.ReceiverAccountId)
            .WithPageSize(1));

        // Act
        Invoice dbInvoice = await _invoiceRepository.QueryAsync(query, default).FirstAsync();

        // Assert
        dbInvoice.Should().BeEquivalentTo(invoice);
    }

    [Fact]
    public async Task QueryAsync_ShouldReturnInvoice_WhenQueryByState()
    {
        // Arrange
        Invoice invoice = await SeedInvoiceAsync(new CreatedInvoiceState());
        var query = InvoiceQuery.Build(builder => builder
            .WithStatus(invoice.State.State)
            .WithPageSize(1));

        var revokedStateQuery = InvoiceQuery.Build(builder => builder
            .WithPageSize(1)
            .WithStatus(InvoiceStatus.Revoked));

        // Act
        Invoice dbInvoice = await _invoiceRepository.QueryAsync(query, default).FirstAsync();
        List<Invoice> emptyInvoices = await _invoiceRepository.QueryAsync(revokedStateQuery, default).ToListAsync();

        // Assert
        dbInvoice.Should().BeEquivalentTo(invoice);
        emptyInvoices.Should().BeEmpty();
    }

    private async Task<Invoice> SeedInvoiceAsync(IInvoiceState state)
    {
        Invoice invoice = new AutoFaker<Invoice>().RuleFor(i => i.State, state).Generate();
        return await _invoiceRepository.AddAsync([invoice], default).FirstAsync();
    }
}