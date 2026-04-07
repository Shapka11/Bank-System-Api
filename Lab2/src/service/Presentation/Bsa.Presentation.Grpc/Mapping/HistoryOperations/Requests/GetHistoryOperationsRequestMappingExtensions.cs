using Bsa.Application.Contracts.HistoryOperations.Operations;
using System.Text.Json;

namespace Bsa.Presentation.Grpc.Mapping.HistoryOperations.Requests;

public static class GetHistoryOperationsRequestMappingExtensions
{
    public static GetHistoryOperations.Request MapToApplication(this ProtoGetHistoryOperationRequest protoRequest)
    {
        GetHistoryOperations.PageToken? pageToken = protoRequest.Pagination.PageToken is null
            ? null
            : JsonSerializer.Deserialize<GetHistoryOperations.PageToken>(protoRequest.Pagination.PageToken);

        return new GetHistoryOperations.Request(
            Guid.Parse(protoRequest.Id),
            protoRequest.Pagination.PageSize,
            pageToken);
    }
}