using BankSystemApi.Application.Contracts.Invoices.Models;
using BankSystemApi.Domain.Invoices;

namespace BankSystemApi.Application.Mapping;

public static class InvoiceMappingExtensions
{
    public static InvoiceDto MapToDto(this Invoice invoice)
        => new(
            invoice.Id.Value,
            invoice.SenderAccountId.Value,
            invoice.ReceiverAccountId.Value,
            invoice.Amount.Value,
            invoice.State.State.MapToDomain(),
            invoice.CreatedAt,
            invoice.UpdatedAt);
}