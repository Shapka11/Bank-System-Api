using BankSystemApi.Gateway.Application.Contracts.Invoices.Models;
using BankSystemApi.Gateway.Presentation.Http.Models.Invoices;

namespace BankSystemApi.Gateway.Presentation.Http.Mapping.Invoices;

public static class InvoiceMappingExtensions
{
    public static InvoiceModel MapToModel(this InvoiceDto dto)
        => new(
            dto.Id,
            dto.SenderAccountId,
            dto.ReceiverAccountId,
            dto.Amount,
            dto.Status.MapToModel(),
            dto.CreatedAt,
            dto.UpdatedAt);
}