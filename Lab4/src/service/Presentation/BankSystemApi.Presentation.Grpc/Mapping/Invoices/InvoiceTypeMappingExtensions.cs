using BankSystemApi.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Presentation.Grpc.Mapping.Invoices;

public static class InvoiceTypeMappingExtensions
{
    public static InvoiceTypeDto MapToDto(this ProtoInvoiceType protoType)
    {
        return protoType switch
        {
            ProtoInvoiceType.Unspecified => throw new ArgumentException("Type is not set", nameof(protoType)),
            ProtoInvoiceType.Incoming => InvoiceTypeDto.Incoming,
            ProtoInvoiceType.Outgoing => InvoiceTypeDto.Outgoing,
            _ => throw new ArgumentOutOfRangeException(nameof(protoType), protoType, "Incorrect type"),
        };
    }
}