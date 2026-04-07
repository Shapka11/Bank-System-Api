using Bsa.Gateway.Application.Abstractions.Invoices.Models;
using Google.Protobuf.Collections;

namespace Bsa.Gateway.Infrastructure.BankService.Mapping;

public static class InvoiceProtoMappingExtensions
{
    public static BankInvoiceModel MapToModel(this ProtoInvoice invoice)
        => new BankInvoiceModel(
            invoice.Id,
            invoice.SenderAccountNumber,
            invoice.ReceiverAccountNumber,
            invoice.Amount.DecimalValue,
            invoice.Status.MapToModel(),
            invoice.CreatedAt.ToDateTimeOffset(),
            invoice.UpdatedAt.ToDateTimeOffset());

    public static IEnumerable<BankInvoiceModel> MapToModel(this RepeatedField<ProtoInvoice> invoices)
        => invoices.Select(MapToModel);
}