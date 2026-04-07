using Bsa.Gateway.Application.Contracts.HistoryOperations.Operations;

namespace Bsa.Gateway.Application.Contracts.HistoryOperations;

public interface IHistoryOperationService
{
    Task<GetHistoryOperationsResponse> GetAsync(
        GetHistoryOperationsRequest request,
        CancellationToken cancellationToken);
}