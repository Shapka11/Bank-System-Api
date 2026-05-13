using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Operations;

namespace BankSystemApi.Gateway.Application.Abstractions.HistoryOperations;

public interface IHistoryOperationClient
{
    Task<GetHistoryOperations.Response> GetAsync(
        GetHistoryOperations.Request request,
        CancellationToken cancellationToken);
}