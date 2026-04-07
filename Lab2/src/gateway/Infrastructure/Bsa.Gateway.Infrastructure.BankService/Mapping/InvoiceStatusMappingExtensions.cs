using Bsa.Gateway.Application.Abstractions.Invoices.Models;
using Google.Protobuf.Collections;

namespace Bsa.Gateway.Infrastructure.BankService.Mapping;

public static class InvoiceStatusMappingExtensions
{
    public static BankInvoiceStatusModel MapToModel(this ProtoInvoiceStatus protoStatus)
    {
        return protoStatus switch
        {
            ProtoInvoiceStatus.Unspecified => throw new ArgumentException("Status is not set", nameof(protoStatus)),
            ProtoInvoiceStatus.Created => BankInvoiceStatusModel.Created,
            ProtoInvoiceStatus.Paid => BankInvoiceStatusModel.Paid,
            ProtoInvoiceStatus.Revoked => BankInvoiceStatusModel.Revoked,
            _ => throw new ArgumentOutOfRangeException(nameof(protoStatus), protoStatus, "Incorrect status"),
        };
    }

    public static ProtoInvoiceStatus MapToProto(this BankInvoiceStatusModel model)
    {
        return model switch
        {
            BankInvoiceStatusModel.Created => ProtoInvoiceStatus.Created,
            BankInvoiceStatusModel.Paid => ProtoInvoiceStatus.Paid,
            BankInvoiceStatusModel.Revoked => ProtoInvoiceStatus.Revoked,
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, "Incorrect status"),
        };
    }

    public static RepeatedField<ProtoInvoiceStatus> MapToProto(this IEnumerable<BankInvoiceStatusModel> models)
    {
        var repeatedField = new RepeatedField<ProtoInvoiceStatus>();
        repeatedField.AddRange(models.Select(m => m.MapToProto()));

        return repeatedField;
    }
}