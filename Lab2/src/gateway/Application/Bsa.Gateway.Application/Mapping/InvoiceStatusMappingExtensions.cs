using Bsa.Gateway.Application.Abstractions.Invoices.Models;
using Bsa.Gateway.Application.Contracts.Invoices.Models;

namespace Bsa.Gateway.Application.Mapping;

public static class InvoiceStatusMappingExtensions
{
    public static InvoiceStatusDto MapToDto(this BankInvoiceStatusModel model)
    {
        return model switch
        {
            BankInvoiceStatusModel.Created => InvoiceStatusDto.Created,
            BankInvoiceStatusModel.Paid => InvoiceStatusDto.Paid,
            BankInvoiceStatusModel.Revoked => InvoiceStatusDto.Revoked,
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, "Incorrect status"),
        };
    }

    public static BankInvoiceStatusModel MapToBankModel(this InvoiceStatusDto dto)
    {
        return dto switch
        {
            InvoiceStatusDto.Created => BankInvoiceStatusModel.Created,
            InvoiceStatusDto.Paid => BankInvoiceStatusModel.Paid,
            InvoiceStatusDto.Revoked => BankInvoiceStatusModel.Revoked,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, "Incorrect status"),
        };
    }
}