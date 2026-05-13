using BankSystemApi.Application.Abstractions.Persistence.Repositories;

namespace BankSystemApi.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    IAccountRepository AccountsRepository { get; }

    IHistoryOperationRepository HistoryOperationsRepository { get; }

    IUserRepository UsersRepository { get; }

    IInvoiceRepository InvoicesRepository { get; }
}