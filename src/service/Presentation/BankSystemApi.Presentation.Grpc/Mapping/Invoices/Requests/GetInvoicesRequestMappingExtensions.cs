using BankSystemApi.Application.Contracts.Invoices.Operations;
using System.Text.Json;

namespace BankSystemApi.Presentation.Grpc.Mapping.Invoices.Requests;

public static class GetInvoicesRequestMappingExtensions
{
    public static GetInvoices.Request MapToApplication(this ProtoGetInvoicesRequest protoRequest)
    {
        GetInvoices.PageToken? pageToken = protoRequest.Pagination.PageToken is null
            ? null
            : JsonSerializer.Deserialize<GetInvoices.PageToken>(protoRequest.Pagination.PageToken);

        return new GetInvoices.Request(
            Guid.Parse(protoRequest.UserId),
            protoRequest.OtherUserAccountIds,
            protoRequest.Statuses.Select(s => s.MapToDto()),
            protoRequest.InvoiceType.MapToDto(),
            protoRequest.Pagination.PageSize,
            pageToken);
    }
}