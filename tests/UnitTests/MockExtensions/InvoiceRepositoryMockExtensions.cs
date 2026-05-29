using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Domain.Invoices;
using Moq;

namespace UnitTests.MockExtensions;

public static class InvoiceRepositoryMockExtensions
{
    public static Mock<IInvoiceRepository> SetupAddInvoice(
        this Mock<IInvoiceRepository> mock,
        Invoice invoice,
        InvoiceId expectedId)
    {
        mock
            .Setup(repo => repo.AddAsync(
                It.Is<IReadOnlyCollection<Invoice>>(c => c.Any(i => i.Id == invoice.Id)),
                It.IsAny<CancellationToken>()))
            .Returns((IReadOnlyCollection<Invoice> list, CancellationToken _) =>
            {
                Invoice first = list.First();
                var result = new Invoice(
                    expectedId,
                    first.SenderAccountId,
                    first.ReceiverAccountId,
                    first.Amount,
                    first.State,
                    first.CreatedAt,
                    first.UpdatedAt);
                return new[] { result }.ToAsyncEnumerable();
            });

        return mock;
    }

    public static Mock<IInvoiceRepository> SetupQueryInvoiceById(
        this Mock<IInvoiceRepository> mock,
        InvoiceId id,
        params Invoice[] returnedAccounts)
    {
        mock
            .Setup(repo => repo.QueryAsync(
                It.Is<InvoiceQuery>(q => q.Ids.Contains(id)),
                It.IsAny<CancellationToken>()))
            .Returns(returnedAccounts.ToAsyncEnumerable());

        return mock;
    }

    public static Mock<IInvoiceRepository> SetupUpdateInvoice(
        this Mock<IInvoiceRepository> mock,
        Invoice invoice)
    {
        mock
            .Setup(repo => repo.UpdateAsync(
                It.Is<IReadOnlyCollection<Invoice>>(c => c.Any(i => i.Id == invoice.Id)),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mock;
    }
}