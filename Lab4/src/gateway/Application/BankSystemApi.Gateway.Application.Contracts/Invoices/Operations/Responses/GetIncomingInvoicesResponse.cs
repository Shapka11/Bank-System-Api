using BankSystemApi.Gateway.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Responses;

public abstract record GetIncomingInvoicesResponse
{
    private GetIncomingInvoicesResponse() { }

    public sealed record Success(
        IReadOnlyCollection<InvoiceDto> Invoices,
        string? PageToken) : GetIncomingInvoicesResponse;

    public sealed record Failure(string ErrorMessage) : GetIncomingInvoicesResponse;
}