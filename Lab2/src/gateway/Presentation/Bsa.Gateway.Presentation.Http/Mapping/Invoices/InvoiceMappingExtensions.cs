using Bsa.Gateway.Application.Contracts.Invoices.Models;
using Bsa.Gateway.Presentation.Http.Models.Invoices;

namespace Bsa.Gateway.Presentation.Http.Mapping.Invoices;

public static class InvoiceMappingExtensions
{
    public static InvoiceModel MapToModel(this InvoiceDto dto)
        => new(
            dto.Id,
            dto.SenderAccountNumber,
            dto.ReceiverAccountNumber,
            dto.Amount,
            dto.Status.MapToModel(),
            dto.CreatedAt,
            dto.UpdatedAt);

    public static IEnumerable<InvoiceModel> MapToModel(this IEnumerable<InvoiceDto> dtos)
        => dtos.Select(MapToModel);
}