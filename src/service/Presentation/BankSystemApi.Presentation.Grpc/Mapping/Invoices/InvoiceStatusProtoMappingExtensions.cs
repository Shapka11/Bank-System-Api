using BankSystemApi.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Presentation.Grpc.Mapping.Invoices;

public static class InvoiceStatusProtoMappingExtensions
{
    public static ProtoInvoiceStatus MapToProto(this InvoiceStatusDto dto)
    {
        return dto switch
        {
            InvoiceStatusDto.Created => ProtoInvoiceStatus.Created,
            InvoiceStatusDto.Paid => ProtoInvoiceStatus.Paid,
            InvoiceStatusDto.Revoked => ProtoInvoiceStatus.Revoked,
            InvoiceStatusDto.Approved => ProtoInvoiceStatus.Approved,
            InvoiceStatusDto.Declined => ProtoInvoiceStatus.Declined,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, "Incorrect status"),
        };
    }

    public static InvoiceStatusDto MapToDto(this ProtoInvoiceStatus protoStatus)
    {
        return protoStatus switch
        {
            ProtoInvoiceStatus.Unspecified => throw new ArgumentException("Status is not set", nameof(protoStatus)),
            ProtoInvoiceStatus.Created => InvoiceStatusDto.Created,
            ProtoInvoiceStatus.Paid => InvoiceStatusDto.Paid,
            ProtoInvoiceStatus.Revoked => InvoiceStatusDto.Revoked,
            ProtoInvoiceStatus.Approved => InvoiceStatusDto.Approved,
            ProtoInvoiceStatus.Declined => InvoiceStatusDto.Declined,
            _ => throw new ArgumentOutOfRangeException(nameof(protoStatus), protoStatus, "Incorrect status"),
        };
    }
}