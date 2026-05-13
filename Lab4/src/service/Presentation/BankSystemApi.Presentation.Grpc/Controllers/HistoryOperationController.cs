using BankSystemApi.Application.Contracts.HistoryOperations;
using BankSystemApi.Application.Contracts.HistoryOperations.Operations;
using BankSystemApi.Grpc;
using BankSystemApi.Presentation.Grpc.Mapping.HistoryOperations;
using BankSystemApi.Presentation.Grpc.Mapping.HistoryOperations.Requests;
using Grpc.Core;
using System.Text.Json;

namespace BankSystemApi.Presentation.Grpc.Controllers;

public sealed class HistoryOperationController : HistoryOperationService.HistoryOperationServiceBase
{
    private readonly IHistoryOperationService _historyOperationService;

    public HistoryOperationController(IHistoryOperationService historyOperationService)
    {
        _historyOperationService = historyOperationService;
    }

    public override async Task<ProtoGetHistoryOperationResponse> Get(
        ProtoGetHistoryOperationRequest request,
        ServerCallContext context)
    {
        GetHistoryOperations.Response applicationResponse = await _historyOperationService.GetAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            GetHistoryOperations.Response.Success success => new ProtoGetHistoryOperationResponse(
                success.History.Select(h => h.MapToProto()),
                success.PageToken is not null ? JsonSerializer.Serialize(success.PageToken.Value) : null),
            GetHistoryOperations.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                $"User {unauthorized.UserId} is not authenticated")),
            GetHistoryOperations.Response.AccountNotFound accountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Account {accountNotFound.AccountId} not found")),
            GetHistoryOperations.Response.Forbidden forbidden => throw new RpcException(new Status(
                StatusCode.PermissionDenied,
                forbidden.ErrorMessage)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }
}