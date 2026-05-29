namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Models;

public enum InvoiceStatusDto
{
    Created = 1,
    Paid,
    Revoked,
    Approved,
    Declined,
}