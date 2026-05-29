using BankSystemApi.Application.Contracts.Accounts.Operations;
using System.Text.Json;

namespace BankSystemApi.Presentation.Grpc.Mapping.Accounts.Requests;

public static class GetAccountsRequestMappingExtensions
{
    public static GetAccounts.Request MapToApplication(this ProtoGetAccountsRequest protoRequest)
    {
        GetAccounts.PageToken? pageToken = protoRequest.Pagination.PageToken is null
            ? null
            : JsonSerializer.Deserialize<GetAccounts.PageToken>(protoRequest.Pagination.PageToken);

        return new GetAccounts.Request(
            Guid.Parse(protoRequest.UserId),
            protoRequest.Pagination.PageSize,
            pageToken);
    }
}