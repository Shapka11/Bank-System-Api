using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations;
using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Operations;
using BankSystemApi.Gateway.Application.Contracts.HistoryOperations;
using BankSystemApi.Gateway.Application.Contracts.HistoryOperations.Operations;
using BankSystemApi.Gateway.Application.Mapping.HistoryOperations;
using System.Diagnostics;

namespace BankSystemApi.Gateway.Application.Services;

internal sealed class HistoryOperationService : IHistoryOperationService
{
    private readonly IHistoryOperationClient _historyOperationClient;

    public HistoryOperationService(IHistoryOperationClient historyOperationClient)
    {
        _historyOperationClient = historyOperationClient;
    }

    public async Task<GetHistoryOperationsResponse> GetAsync(
        GetHistoryOperationsRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new GetHistoryOperations.Request(
            request.UserId,
            request.AccountId,
            request.PageSize,
            request.PageToken);
        GetHistoryOperations.Response clientResponse = await _historyOperationClient
            .GetAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            GetHistoryOperations.Response.Failure failure
                => new GetHistoryOperationsResponse.Failure(failure.ErrorMessage),
            GetHistoryOperations.Response.Success success
                => new GetHistoryOperationsResponse.Success(
                    success.History.Select(h => h.MapToDto()).ToArray(),
                    success.PageToken),
            _ => throw new UnreachableException(),
        };
    }
}