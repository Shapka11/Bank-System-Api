using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Domain.HistoryOperations;

namespace BankSystemApi.Application.Abstractions.Persistence.Repositories;

public interface IHistoryOperationRepository
{
    IAsyncEnumerable<HistoryOperation> AddAsync(
        IReadOnlyCollection<HistoryOperation> historyOperations,
        CancellationToken cancellationToken);

    IAsyncEnumerable<HistoryOperation> QueryAsync(OperationHistoryQuery historyQuery, CancellationToken cancellationToken);
}