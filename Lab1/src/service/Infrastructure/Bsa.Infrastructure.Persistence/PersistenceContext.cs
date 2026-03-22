using Bsa.Application.Abstractions.Persistence;
using Bsa.Application.Abstractions.Persistence.Repositories;

namespace Bsa.Infrastructure.Persistence;

public sealed class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(
        IAccountRepository accounts,
        IAccountOperationRepository accountOperations,
        IUserSessionRepository userSessions,
        IAdminSessionRepository adminSessions)
    {
        AccountsRepository = accounts;
        AccountOperationsRepository = accountOperations;
        UserSessionsRepository = userSessions;
        AdminSessionsRepository = adminSessions;
    }

    public IAccountRepository AccountsRepository { get; }

    public IAccountOperationRepository AccountOperationsRepository { get; }

    public IUserSessionRepository UserSessionsRepository { get; }

    public IAdminSessionRepository AdminSessionsRepository { get; }
}