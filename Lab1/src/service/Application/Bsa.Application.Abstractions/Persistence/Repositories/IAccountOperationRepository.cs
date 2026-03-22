using Bsa.Domain.Operations;
using AccountOperationQuery = Bsa.Application.Abstractions.Persistence.Queries.AccountOperationQuery;

namespace Bsa.Application.Abstractions.Persistence.Repositories;

public interface IAccountOperationRepository
{
    IAsyncEnumerable<AccountOperation> AddAsync(
        IReadOnlyCollection<AccountOperation> accountOperations,
        CancellationToken cancellationToken);

    IAsyncEnumerable<AccountOperation> QueryAsync(AccountOperationQuery query, CancellationToken cancellationToken);
}