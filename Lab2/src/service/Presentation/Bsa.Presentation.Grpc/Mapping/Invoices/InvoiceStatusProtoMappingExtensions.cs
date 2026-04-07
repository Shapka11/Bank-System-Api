using Bsa.Application.Contracts.Invoices.Models;
using Google.Protobuf.Collections;

namespace Bsa.Presentation.Grpc.Mapping.Invoices;

public static class InvoiceStatusProtoMappingExtensions
{
    public static ProtoInvoiceStatus MapToProto(this InvoiceStatusDto dto)
    {
        return dto switch
        {
            InvoiceStatusDto.Created => ProtoInvoiceStatus.Created,
            InvoiceStatusDto.Paid => ProtoInvoiceStatus.Paid,
            InvoiceStatusDto.Revoked => ProtoInvoiceStatus.Revoked,
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
            _ => throw new ArgumentOutOfRangeException(nameof(protoStatus), protoStatus, "Incorrect status"),
        };
    }

    public static IEnumerable<InvoiceStatusDto> MapToDto(this RepeatedField<ProtoInvoiceStatus> protoStatuses)
        => protoStatuses.Select(MapToDto);
}