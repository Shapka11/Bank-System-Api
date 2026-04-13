using Bsa.Application.Contracts.Invoices.Operations;
using System.Text.Json;

namespace Bsa.Presentation.Grpc.Mapping.Invoices.Requests;

public static class GetOutgoingInvoicesMappingExtensions
{
    public static GetOutgoingInvoices.Request MapToApplication(this ProtoGetOutgoingInvoicesRequest protoRequest)
    {
        GetOutgoingInvoices.PageToken? pageToken = protoRequest.Pagination.PageToken is null
            ? null
            : JsonSerializer.Deserialize<GetOutgoingInvoices.PageToken>(protoRequest.Pagination.PageToken);

        return new GetOutgoingInvoices.Request(
            Guid.Parse(protoRequest.SessionId),
            protoRequest.ReceiverAccountNumbers,
            protoRequest.Statuses.Select(s => s.MapToDto()),
            protoRequest.Pagination.PageSize,
            pageToken);
    }
}