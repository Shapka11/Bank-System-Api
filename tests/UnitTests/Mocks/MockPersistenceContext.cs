using BankSystemApi.Application.Abstractions.Persistence;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using Moq;

namespace UnitTests.Mocks;

public sealed class MockPersistenceContext : IPersistenceContext
{
    public Mock<IAccountRepository> AccountsRepository { get; } = new(MockBehavior.Strict);

    public Mock<IHistoryOperationRepository> HistoryOperationsRepository { get; } = new(MockBehavior.Strict);

    public Mock<IInvoiceRepository> InvoicesRepository { get; } = new(MockBehavior.Strict);

    public Mock<IUserRepository> UsersRepository { get; } = new(MockBehavior.Strict);

    IAccountRepository IPersistenceContext.AccountsRepository => AccountsRepository.Object;

    IHistoryOperationRepository IPersistenceContext.HistoryOperationsRepository => HistoryOperationsRepository.Object;

    IInvoiceRepository IPersistenceContext.InvoicesRepository => InvoicesRepository.Object;

    IUserRepository IPersistenceContext.UsersRepository => UsersRepository.Object;

    public void VerifyAll()
    {
        AccountsRepository.VerifyAll();
        HistoryOperationsRepository.VerifyAll();
        InvoicesRepository.VerifyAll();
        UsersRepository.VerifyAll();
    }
}