using Bsa.Gateway.Application.Contracts.Invoices.Operations.Requests;
using Bsa.Gateway.Presentation.Http.Requests.Invoices;

namespace Bsa.Gateway.Presentation.Http.Mapping.Invoices.Requests;

public static class GetOutgoingInvoicesRequestMappingExtensions
{
    public static GetOutgoingInvoicesRequest MapToApplication(this GetOutgoingInvoicesHttpRequest httpRequest)
        => new(
            httpRequest.SessionId,
            httpRequest.ReceiverAccountNumbers,
            httpRequest.Statuses.Select(s => s.MapToDto()),
            httpRequest.PageSize,
            httpRequest.PageToken);
}