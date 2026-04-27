using BankSystemApi.Gateway.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Responses;

public abstract record RevokeInvoiceResponse
{
    private RevokeInvoiceResponse() { }

    public sealed record Success(InvoiceDto Invoice) : RevokeInvoiceResponse;

    public sealed record Failure(string ErrorMessage) : RevokeInvoiceResponse;
}