using Bsa.Gateway.Application.Contracts.Invoices.Operations.Requests;
using Bsa.Gateway.Presentation.Http.Requests.Invoices;

namespace Bsa.Gateway.Presentation.Http.Mapping.Invoices.Requests;

public static class PayInvoiceRequestMappingExtensions
{
    public static PayInvoiceRequest MapToApplication(this PayInvoiceHttpRequest httpRequest)
        => new(httpRequest.SessionId, httpRequest.InvoiceId);
}