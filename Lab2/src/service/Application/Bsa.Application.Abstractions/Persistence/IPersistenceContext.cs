using Bsa.Application.Abstractions.Persistence.Repositories;

namespace Bsa.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    IAccountRepository AccountsRepository { get; }

    IHistoryOperationRepository HistoryOperationsRepository { get; }

    ISessionRepository SessionRepository { get; }

    IInvoiceRepository InvoiceRepository { get; }
}