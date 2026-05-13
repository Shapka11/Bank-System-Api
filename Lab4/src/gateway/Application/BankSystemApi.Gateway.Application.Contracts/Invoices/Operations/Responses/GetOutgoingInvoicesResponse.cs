using BankSystemApi.Gateway.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Gateway.Application.Contracts.Invoices.Operations.Responses;

public abstract record GetOutgoingInvoicesResponse
{
    private GetOutgoingInvoicesResponse() { }

    public sealed record Success(
        IReadOnlyCollection<InvoiceDto> Invoices,
        string? PageToken) : GetOutgoingInvoicesResponse;

    public sealed record Failure(string ErrorMessage) : GetOutgoingInvoicesResponse;
}