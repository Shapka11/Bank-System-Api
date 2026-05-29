using BankSystemApi.Gateway.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Responses;

public abstract record PayInvoiceResponse
{
    private PayInvoiceResponse() { }

    public sealed record Success(InvoiceDto Invoice) : PayInvoiceResponse;

    public sealed record Failure(string ErrorMessage) : PayInvoiceResponse;
}