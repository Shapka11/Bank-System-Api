using BankSystemApi.Application.Abstractions.Persistence;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;

namespace BankSystemApi.Infrastructure.Persistence;

public sealed class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(
        IAccountRepository accounts,
        IHistoryOperationRepository historyOperations,
        IInvoiceRepository invoiceRepository,
        IUserRepository userRepository)
    {
        AccountsRepository = accounts;
        HistoryOperationsRepository = historyOperations;
        InvoiceRepository = invoiceRepository;
        UserRepository = userRepository;
    }

    public IAccountRepository AccountsRepository { get; }

    public IHistoryOperationRepository HistoryOperationsRepository { get; }

    public IUserRepository UserRepository { get; }

    public IInvoiceRepository InvoiceRepository { get; }
}