using BankSystemApi.Application.Contracts.Invoices.Operations;

namespace BankSystemApi.Presentation.Grpc.Mapping.Invoices.Requests;

public static class RevokeInvoiceRequestMappingExtensions
{
    public static RevokeInvoice.Request MapToApplication(this ProtoRevokeInvoiceRequest protoRequest)
        => new(Guid.Parse(protoRequest.UserId), protoRequest.InvoiceId);
}