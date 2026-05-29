using BankSystemApi.Application.Contracts.HistoryOperations.Operations;
using System.Text.Json;

namespace BankSystemApi.Presentation.Grpc.Mapping.HistoryOperations.Requests;

public static class GetHistoryOperationsRequestMappingExtensions
{
    public static GetHistoryOperations.Request MapToApplication(this ProtoGetHistoryOperationsRequest protoRequest)
    {
        GetHistoryOperations.PageToken? pageToken = protoRequest.Pagination.PageToken is null
            ? null
            : JsonSerializer.Deserialize<GetHistoryOperations.PageToken>(protoRequest.Pagination.PageToken);

        return new GetHistoryOperations.Request(
            Guid.Parse(protoRequest.UserId),
            protoRequest.AccountId,
            protoRequest.Pagination.PageSize,
            pageToken);
    }
}