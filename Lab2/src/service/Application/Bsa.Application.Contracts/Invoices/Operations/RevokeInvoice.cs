using Bsa.Application.Contracts.Invoices.Models;

namespace Bsa.Application.Contracts.Invoices.Operations;

public static class RevokeInvoice
{
    public readonly record struct Request(Guid SessionId, long InvoiceId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(InvoiceDto Invoice) : Response;

        public sealed record Unauthorized(Guid SessionId, string ErrorMessage) : Response;

        public sealed record InvoiceNotFound(long InvoiceId) : Response;

        public sealed record InvalidInvoiceState(string Message) : Response;

        public sealed record AccountNotFound(string AccountNumber) : Response;

        public sealed record Forbidden(string Message) : Response;
    }
}