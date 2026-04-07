using Bsa.Application.Contracts.Invoices.Models;
using Bsa.Domain.Invoices.States;

namespace Bsa.Application.Mapping;

public static class InvoiceStatusMappingExtensions
{
    public static InvoiceStatusDto MapToDomain(this InvoiceStatus status)
    {
        return status switch
        {
            InvoiceStatus.Created => InvoiceStatusDto.Created,
            InvoiceStatus.Paid => InvoiceStatusDto.Paid,
            InvoiceStatus.Revoked => InvoiceStatusDto.Revoked,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Incorrect status"),
        };
    }

    public static InvoiceStatus MapToDomain(this InvoiceStatusDto dto)
    {
        return dto switch
        {
            InvoiceStatusDto.Created => InvoiceStatus.Created,
            InvoiceStatusDto.Paid => InvoiceStatus.Paid,
            InvoiceStatusDto.Revoked => InvoiceStatus.Revoked,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, "Incorrect status"),
        };
    }

    public static IEnumerable<InvoiceStatus> MapToDomain(this IEnumerable<InvoiceStatusDto> dtos)
        => dtos.Select(MapToDomain);
}