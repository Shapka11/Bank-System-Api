using Bsa.Gateway.Application.Abstractions.Invoices.Models;

namespace Bsa.Gateway.Infrastructure.BankService.Mapping;

public static class InvoiceProtoMappingExtensions
{
    public static BankInvoiceModel MapToModel(this ProtoInvoice invoice)
        => new(
            invoice.Id,
            invoice.SenderAccountNumber,
            invoice.ReceiverAccountNumber,
            invoice.Amount.DecimalValue,
            invoice.Status.MapToModel(),
            invoice.CreatedAt.ToDateTimeOffset(),
            invoice.UpdatedAt.ToDateTimeOffset());
}