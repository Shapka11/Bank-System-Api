using BankSystemApi.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Application.Contracts.Invoices.Operations;

public static class ApproveInvoice
{
    public readonly record struct Request(long InvoiceId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(InvoiceDto Invoice) : Response;

        public sealed record InvoiceNotFound(long InvoiceId) : Response;

        public sealed record AccountNotFound(long AccountId) : Response;

        public sealed record InvalidInvoiceState(string State) : Response;
    }
}