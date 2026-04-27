using BankSystemApi.Application.Contracts.HistoryOperations.Operations;

namespace BankSystemApi.Application.Contracts.HistoryOperations;

public interface IHistoryOperationService
{
    Task<GetHistoryOperations.Response> GetAsync(
        GetHistoryOperations.Request request,
        CancellationToken cancellationToken);
}