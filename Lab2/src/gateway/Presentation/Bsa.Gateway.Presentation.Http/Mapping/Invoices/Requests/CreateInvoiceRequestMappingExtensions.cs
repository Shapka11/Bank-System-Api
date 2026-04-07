using Bsa.Gateway.Application.Contracts.Invoices.Operations.Requests;
using Bsa.Gateway.Presentation.Http.Requests.Invoices;

namespace Bsa.Gateway.Presentation.Http.Mapping.Invoices.Requests;

public static class CreateInvoiceRequestMappingExtensions
{
    public static CreateInvoiceRequest MapToApplication(this CreateInvoiceHttpRequest httpRequest)
        => new(
            httpRequest.SessionId,
            httpRequest.SenderAccountNumber,
            httpRequest.ReceiverAccountNumber,
            httpRequest.Amount);
}