using Bsa.Gateway.Application.Abstractions.HistoryOperations;
using Bsa.Gateway.Application.Abstractions.HistoryOperations.Operations;
using Bsa.Gateway.Application.Contracts.HistoryOperations;
using Bsa.Gateway.Application.Contracts.HistoryOperations.Operations;
using Bsa.Gateway.Application.Mapping;
using System.Diagnostics;

namespace Bsa.Gateway.Application.Services;

public sealed class HistoryOperationService : IHistoryOperationService
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
            request.Id,
            request.PageSize,
            request.PageToken);
        GetHistoryOperations.Response clientResponse = await _historyOperationClient
            .GetAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            GetHistoryOperations.Response.Failure failure
                => new GetHistoryOperationsResponse.Failure(failure.ErrorMessage),
            GetHistoryOperations.Response.Success success
                => new GetHistoryOperationsResponse.Success(success.History.MapToDto(), success.PageToken),
            _ => throw new UnreachableException(),
        };
    }
}