using BankSystemApi.Gateway.Application.Abstractions.Invoices.Models;
using BankSystemApi.Gateway.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Gateway.Application.Mapping.Invoices;

public static class InvoiceMappingExtensions
{
    public static InvoiceDto MapToDto(this BankInvoiceModel model)
        => new(
            model.Id,
            model.SenderAccountId,
            model.ReceiverAccountId,
            model.Amount,
            model.Status.MapToDto(),
            model.CreatedAt,
            model.UpdatedAt);
}