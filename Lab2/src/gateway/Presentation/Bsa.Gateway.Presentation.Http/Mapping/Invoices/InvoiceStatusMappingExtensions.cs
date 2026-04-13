using Bsa.Gateway.Application.Contracts.Invoices.Models;
using Bsa.Gateway.Presentation.Http.Models.Invoices;

namespace Bsa.Gateway.Presentation.Http.Mapping.Invoices;

public static class InvoiceStatusMappingExtensions
{
    public static InvoiceStatusModel MapToModel(this InvoiceStatusDto dto)
    {
        return dto switch
        {
            InvoiceStatusDto.Created => InvoiceStatusModel.Created,
            InvoiceStatusDto.Paid => InvoiceStatusModel.Paid,
            InvoiceStatusDto.Revoked => InvoiceStatusModel.Revoked,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, "Incorrect status"),
        };
    }

    public static InvoiceStatusDto MapToDto(this InvoiceStatusModel model)
    {
        return model switch
        {
            InvoiceStatusModel.Created => InvoiceStatusDto.Created,
            InvoiceStatusModel.Paid => InvoiceStatusDto.Paid,
            InvoiceStatusModel.Revoked => InvoiceStatusDto.Revoked,
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, "Incorrect status"),
        };
    }
}