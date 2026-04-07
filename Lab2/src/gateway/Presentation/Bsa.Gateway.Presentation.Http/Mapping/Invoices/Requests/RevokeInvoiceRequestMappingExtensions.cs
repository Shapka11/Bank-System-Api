using Bsa.Gateway.Application.Contracts.Invoices.Operations.Requests;
using Bsa.Gateway.Presentation.Http.Requests.Invoices;

namespace Bsa.Gateway.Presentation.Http.Mapping.Invoices.Requests;

public static class RevokeInvoiceRequestMappingExtensions
{
    public static RevokeInvoiceRequest MapToApplication(this RevokeInvoiceHttpRequest httpRequest)
        => new(httpRequest.SessionId, httpRequest.InvoiceId);
}