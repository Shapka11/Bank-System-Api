using Bsa.Application.Abstractions.Persistence;
using Bsa.Application.Abstractions.Persistence.Repositories;

namespace Bsa.Infrastructure.Persistence;

public sealed class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(
        IAccountRepository accounts,
        IHistoryOperationRepository historyOperations,
        IInvoiceRepository invoiceRepository,
        ISessionRepository sessionRepository)
    {
        AccountsRepository = accounts;
        HistoryOperationsRepository = historyOperations;
        InvoiceRepository = invoiceRepository;
        SessionRepository = sessionRepository;
    }

    public IAccountRepository AccountsRepository { get; }

    public IHistoryOperationRepository HistoryOperationsRepository { get; }

    public ISessionRepository SessionRepository { get; }

    public IInvoiceRepository InvoiceRepository { get; }
}