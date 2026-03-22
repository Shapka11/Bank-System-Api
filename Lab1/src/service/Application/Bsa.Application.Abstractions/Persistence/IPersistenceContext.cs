using Bsa.Application.Abstractions.Persistence.Repositories;

namespace Bsa.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    IAccountRepository AccountsRepository { get; }

    IAccountOperationRepository AccountOperationsRepository { get; }

    IUserSessionRepository UserSessionsRepository { get; }

    IAdminSessionRepository AdminSessionsRepository { get; }
}