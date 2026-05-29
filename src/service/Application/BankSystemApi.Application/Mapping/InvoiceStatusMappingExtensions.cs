using BankSystemApi.Application.Contracts.Invoices.Models;
using BankSystemApi.Domain.Invoices.States;

namespace BankSystemApi.Application.Mapping;

public static class InvoiceStatusMappingExtensions
{
    public static InvoiceStatusDto MapToDto(this InvoiceStatus status)
    {
        return status switch
        {
            InvoiceStatus.Created => InvoiceStatusDto.Created,
            InvoiceStatus.Paid => InvoiceStatusDto.Paid,
            InvoiceStatus.Revoked => InvoiceStatusDto.Revoked,
            InvoiceStatus.Approved => InvoiceStatusDto.Approved,
            InvoiceStatus.Declined => InvoiceStatusDto.Declined,
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
            InvoiceStatusDto.Approved => InvoiceStatus.Approved,
            InvoiceStatusDto.Declined => InvoiceStatus.Declined,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, "Incorrect status"),
        };
    }
}