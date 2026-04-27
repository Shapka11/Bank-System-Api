using BankSystemApi.Application.Contracts.Invoices.Models;
using Google.Protobuf.WellKnownTypes;
using Google.Type;

namespace BankSystemApi.Presentation.Grpc.Mapping.Invoices;

public static class InvoiceProtoMappingExtensions
{
    public static ProtoInvoice MapToProto(this InvoiceDto dto) =>
        new(
            dto.Id,
            dto.SenderAccountId.ToString(),
            dto.ReceiverAccountId.ToString(),
            new Money { DecimalValue = dto.Amount },
            dto.Status.MapToProto(),
            dto.CreatedAt.ToTimestamp(),
            dto.UpdatedAt.ToTimestamp());
}