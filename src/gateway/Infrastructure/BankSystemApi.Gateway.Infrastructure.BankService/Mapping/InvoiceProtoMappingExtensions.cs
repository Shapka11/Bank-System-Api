using BankSystemApi.Gateway.Application.Abstractions.Invoices.Models;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Mapping;

public static class InvoiceProtoMappingExtensions
{
    public static BankInvoiceModel MapToModel(this ProtoInvoice invoice)
        => new(
            invoice.Id,
            invoice.SenderAccountId,
            invoice.ReceiverAccountId,
            invoice.Amount.DecimalValue,
            invoice.Status.MapToModel(),
            invoice.CreatedAt.ToDateTimeOffset(),
            invoice.UpdatedAt.ToDateTimeOffset());
}