using Bsa.Gateway.Application.Abstractions.Invoices.Models;
using Bsa.Gateway.Application.Contracts.Invoices.Models;

namespace Bsa.Gateway.Application.Mapping;

public static class InvoiceMappingExtensions
{
    public static InvoiceDto MapToDto(this BankInvoiceModel model)
        => new InvoiceDto(
            model.Id,
            model.SenderAccountNumber,
            model.ReceiverAccountNumber,
            model.Amount,
            model.Status.MapToDto(),
            model.CreatedAt,
            model.UpdatedAt);

    public static IEnumerable<InvoiceDto> MapToDto(this IEnumerable<BankInvoiceModel> entities)
        => entities.Select(MapToDto);
}