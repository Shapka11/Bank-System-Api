using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Domain.HistoryOperations;

namespace Bsa.Application.Abstractions.Persistence.Repositories;

public interface IHistoryOperationRepository
{
    IAsyncEnumerable<HistoryOperation> AddAsync(
        IReadOnlyCollection<HistoryOperation> historyOperations,
        CancellationToken cancellationToken);

    IAsyncEnumerable<HistoryOperation> QueryAsync(AccountOperationQuery query, CancellationToken cancellationToken);
}