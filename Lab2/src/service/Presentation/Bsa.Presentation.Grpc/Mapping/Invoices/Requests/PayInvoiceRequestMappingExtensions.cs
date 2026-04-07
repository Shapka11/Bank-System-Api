using Bsa.Application.Contracts.Invoices.Operations;

namespace Bsa.Presentation.Grpc.Mapping.Invoices.Requests;

public static class PayInvoiceRequestMappingExtensions
{
    public static PayInvoice.Request MapToApplication(this ProtoPayInvoiceRequest protoRequest)
        => new(Guid.Parse(protoRequest.SessionId), protoRequest.InvoiceId);
}