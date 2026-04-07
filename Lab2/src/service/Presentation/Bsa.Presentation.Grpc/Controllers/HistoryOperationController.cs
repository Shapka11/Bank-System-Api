using Bsa.Application.Contracts.HistoryOperations;
using Bsa.Application.Contracts.HistoryOperations.Operations;
using Bsa.CsharpBackend.Grpc;
using Bsa.Presentation.Grpc.Mapping.HistoryOperations;
using Bsa.Presentation.Grpc.Mapping.HistoryOperations.Requests;
using Grpc.Core;
using System.Text.Json;

namespace Bsa.Presentation.Grpc.Controllers;

public sealed class HistoryOperationController : HistoryOperationService.HistoryOperationServiceBase
{
    private readonly IHistoryOperationService _historyOperationService;

    public HistoryOperationController(IHistoryOperationService historyOperationService)
    {
        _historyOperationService = historyOperationService;
    }

    public override async Task<ProtoGetHistoryOperationResponse> GetHistoryOperation(
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
                unauthorized.ErrorMessage)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }
}