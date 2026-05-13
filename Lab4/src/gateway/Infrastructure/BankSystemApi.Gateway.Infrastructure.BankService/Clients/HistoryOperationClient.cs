using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations;
using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Operations;
using BankSystemApi.Gateway.Infrastructure.BankService.Activities;
using BankSystemApi.Gateway.Infrastructure.BankService.Extensions;
using BankSystemApi.Gateway.Infrastructure.BankService.Mapping;
using BankSystemApi.Grpc;
using System.Diagnostics;

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
        using Activity? activity = HistoryOperationClientActivity.ActivitySource.StartActivity();
        activity.AddUserIdBaggage(request.UserId);
        activity.AddAccountIdBaggage(request.AccountId);

        var clientRequest = new ProtoGetHistoryOperationRequest(
            request.UserId.ToString(),
            request.AccountId.ToString(),
            new ProtoPagination(request.PageSize, request.PageToken));

        ProtoGetHistoryOperationResponse clientResponse = await _historyClient.GetAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new GetHistoryOperations.Response.Success(
            clientResponse.History.Select(h => h.MapToModel()).ToArray(),
            clientResponse.PageToken);
    }
}