using Bsa.Gateway.Application.Contracts.Invoices.Operations.Requests;
using Bsa.Gateway.Presentation.Http.Requests.Invoices;

namespace Bsa.Gateway.Presentation.Http.Mapping.Invoices.Requests;

public static class GetIncomingInvoicesRequestMappingExtensions
{
    public static GetIncomingInvoicesRequest MapToApplication(this GetIncomingInvoicesHttpRequest httpRequest)
        => new(
            httpRequest.SessionId,
            httpRequest.SenderAccountNumbers,
            httpRequest.Statuses.MapToDto(),
            httpRequest.PageSize,
            httpRequest.PageToken);
}