using BankSystemApi.Application.Contracts.Invoices.Operations;

namespace BankSystemApi.Presentation.Grpc.Mapping.Invoices.Requests;

public static class CreateInvoiceRequestMappingExtensions
{
    public static CreateInvoice.Request MapToApplication(this ProtoCreateInvoiceRequest protoRequest)
        => new(
            Guid.Parse(protoRequest.UserId),
            protoRequest.SenderAccountId,
            protoRequest.ReceiverAccountId,
            protoRequest.Amount.DecimalValue);
}