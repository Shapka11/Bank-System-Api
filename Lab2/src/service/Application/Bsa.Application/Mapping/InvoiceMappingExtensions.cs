using Bsa.Application.Contracts.Invoices.Models;
using Bsa.Domain.Invoices;

namespace Bsa.Application.Mapping;

public static class InvoiceMappingExtensions
{
    public static InvoiceDto MapToDto(this Invoice invoice)
        => new InvoiceDto(
            invoice.Id.Value,
            invoice.SenderAccountNumber.Value,
            invoice.ReceiverAccountNumber.Value,
            invoice.Amount.Value,
            invoice.State.State.MapToDomain(),
            invoice.CreatedAt,
            invoice.UpdatedAt);

    public static IEnumerable<InvoiceDto> MapToDto(this Invoice[] invoices)
        => invoices.Select(operation => operation.MapToDto());
}