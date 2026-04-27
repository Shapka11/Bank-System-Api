using BankSystemApi.Application.Contracts.Invoices.Operations;

namespace BankSystemApi.Presentation.Grpc.Mapping.Invoices.Requests;

public static class PayInvoiceRequestMappingExtensions
{
    public static PayInvoice.Request MapToApplication(this ProtoPayInvoiceRequest protoRequest)
        => new(Guid.Parse(protoRequest.UserId), protoRequest.InvoiceId);
}