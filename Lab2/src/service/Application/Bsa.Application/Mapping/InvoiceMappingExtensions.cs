using Bsa.Application.Contracts.Invoices.Models;
using Bsa.Domain.Invoices;

namespace Bsa.Application.Mapping;

public static class InvoiceMappingExtensions
{
    public static InvoiceDto MapToDto(this Invoice invoice)
        => new(
            invoice.Id.Value,
            invoice.SenderAccountNumber.Value,
            invoice.ReceiverAccountNumber.Value,
            invoice.Amount.Value,
            invoice.State.State.MapToDomain(),
            invoice.CreatedAt,
            invoice.UpdatedAt);
}