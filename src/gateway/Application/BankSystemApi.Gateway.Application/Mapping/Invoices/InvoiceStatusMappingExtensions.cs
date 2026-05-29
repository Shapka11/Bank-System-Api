using BankSystemApi.Gateway.Application.Abstractions.Invoices.Models;
using BankSystemApi.Gateway.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Gateway.Application.Mapping.Invoices;

public static class InvoiceStatusMappingExtensions
{
    public static InvoiceStatusDto MapToDto(this BankInvoiceStatusModel model)
    {
        return model switch
        {
            BankInvoiceStatusModel.Created => InvoiceStatusDto.Created,
            BankInvoiceStatusModel.Paid => InvoiceStatusDto.Paid,
            BankInvoiceStatusModel.Revoked => InvoiceStatusDto.Revoked,
            BankInvoiceStatusModel.Approved => InvoiceStatusDto.Approved,
            BankInvoiceStatusModel.Declined => InvoiceStatusDto.Declined,
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
            InvoiceStatusDto.Approved => BankInvoiceStatusModel.Approved,
            InvoiceStatusDto.Declined => BankInvoiceStatusModel.Declined,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, "Incorrect status"),
        };
    }
}