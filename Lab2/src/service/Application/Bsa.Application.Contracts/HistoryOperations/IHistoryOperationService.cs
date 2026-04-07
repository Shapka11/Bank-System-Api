using Bsa.Application.Contracts.HistoryOperations.Operations;

namespace Bsa.Application.Contracts.HistoryOperations;

public interface IHistoryOperationService
{
    Task<GetHistoryOperations.Response> GetAsync(
        GetHistoryOperations.Request request,
        CancellationToken cancellationToken);
}