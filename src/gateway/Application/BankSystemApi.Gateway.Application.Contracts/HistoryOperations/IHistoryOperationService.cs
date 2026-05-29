using BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Operations;

namespace BankSystemApi.Gateway.Application.Contracts.HistoryOperations;

public interface IHistoryOperationService
{
    Task<GetHistoryOperationsResponse> GetAsync(
        GetHistoryOperationsRequest request,
        CancellationToken cancellationToken);
}