using BankSystemApi.Application.Abstractions.Persistence.Repositories;

namespace BankSystemApi.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    IAccountRepository AccountsRepository { get; }

    IHistoryOperationRepository HistoryOperationsRepository { get; }

    IUserRepository UserRepository { get; }

    IInvoiceRepository InvoiceRepository { get; }
}