using Bsa.Application.Contracts.Invoices.Operations;

namespace Bsa.Presentation.Grpc.Mapping.Invoices.Requests;

public static class RevokeInvoiceRequestMappingExtensions
{
    public static RevokeInvoice.Request MapToApplication(this ProtoRevokeInvoiceRequest protoRequest)
        => new(Guid.Parse(protoRequest.SessionId), protoRequest.InvoiceId);
}