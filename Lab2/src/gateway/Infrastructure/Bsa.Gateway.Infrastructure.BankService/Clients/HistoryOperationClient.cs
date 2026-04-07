using Bsa.CsharpBackend.Grpc;
using Bsa.Gateway.Application.Abstractions.HistoryOperations;
using Bsa.Gateway.Application.Abstractions.HistoryOperations.Operations;
using Bsa.Gateway.Infrastructure.BankService.Mapping;

namespace Bsa.Gateway.Infrastructure.BankService.Clients;

public sealed class HistoryOperationClient : IHistoryOperationClient
{
    private readonly HistoryOperationService.HistoryOperationServiceClient _historyClient;

    public HistoryOperationClient(HistoryOperationService.HistoryOperationServiceClient historyClient)
    {
        _historyClient = historyClient;
    }

    public async Task<GetHistoryOperations.Response> GetAsync(
        GetHistoryOperations.Request request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new ProtoGetHistoryOperationRequest(
            request.Id.ToString(),
            new ProtoPagination(request.PageSize, request.PageToken));

        ProtoGetHistoryOperationResponse clientResponse = await _historyClient.GetHistoryOperationAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new GetHistoryOperations.Response.Success(clientResponse.History.MapToModel(), clientResponse.PageToken);
    }
}