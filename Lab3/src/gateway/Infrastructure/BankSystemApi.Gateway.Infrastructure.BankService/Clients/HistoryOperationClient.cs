using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations;
using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Operations;
using BankSystemApi.Gateway.Infrastructure.BankService.Mapping;
using BankSystemApi.Grpc;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Clients;

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
            request.UserId.ToString(),
            request.AccountId.ToString(),
            new ProtoPagination(request.PageSize, request.PageToken));

        ProtoGetHistoryOperationResponse clientResponse = await _historyClient.GetHistoryOperationAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new GetHistoryOperations.Response.Success(
            clientResponse.History.Select(h => h.MapToModel()).ToArray(),
            clientResponse.PageToken);
    }
}