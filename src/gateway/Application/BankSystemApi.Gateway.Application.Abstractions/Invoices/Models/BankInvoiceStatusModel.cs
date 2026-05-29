namespace BankSystemApi.Gateway.Application.Abstractions.Invoices.Models;

public enum BankInvoiceStatusModel
{
    Created = 1,
    Paid,
    Revoked,
    Approved,
    Declined,
}