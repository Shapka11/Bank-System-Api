using Bsa.Application.Contracts.Invoices.Operations;
using System.Text.Json;

namespace Bsa.Presentation.Grpc.Mapping.Invoices.Requests;

public static class GetIncomingInvoicesRequestMappingExtensions
{
    public static GetIncomingInvoices.Request MapToApplication(this ProtoGetIncomingInvoicesRequest protoRequest)
    {
        GetIncomingInvoices.PageToken? pageToken = protoRequest.Pagination.PageToken is null
            ? null
            : JsonSerializer.Deserialize<GetIncomingInvoices.PageToken>(protoRequest.Pagination.PageToken);

        return new GetIncomingInvoices.Request(
            Guid.Parse(protoRequest.SessionId),
            protoRequest.SenderAccountNumbers,
            protoRequest.Statuses.Select(s => s.MapToDto()),
            protoRequest.Pagination.PageSize,
            pageToken);
    }
}