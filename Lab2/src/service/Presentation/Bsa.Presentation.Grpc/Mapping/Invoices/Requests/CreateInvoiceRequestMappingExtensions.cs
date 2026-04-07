using Bsa.Application.Contracts.Invoices.Operations;

namespace Bsa.Presentation.Grpc.Mapping.Invoices.Requests;

public static class CreateInvoiceRequestMappingExtensions
{
    public static CreateInvoice.Request MapToApplication(this ProtoCreateInvoiceRequest protoRequest)
        => new(
            Guid.Parse(protoRequest.SessionId),
            protoRequest.SenderAccountNumber,
            protoRequest.ReceiverAccountNumber,
            protoRequest.Amount.DecimalValue);
}