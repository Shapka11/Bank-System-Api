using Bsa.Application.Contracts.Invoices.Models;

namespace Bsa.Application.Contracts.Invoices.Operations;

public static class PayInvoice
{
    public readonly record struct Request(Guid SessionId, long InvoiceId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(InvoiceDto Invoice) : Response;

        public sealed record Unauthorized(Guid SessionId, string ErrorMessage) : Response;

        public sealed record InvoiceNotFound(long InvoiceId) : Response;

        public sealed record InvalidInvoiceState(string State) : Response;

        public sealed record AccountNotFound(string AccountNumber) : Response;

        public sealed record Forbidden(string Message) : Response;

        public sealed record InsufficientFunds(string AccountNumber, string Message) : Response;

        public sealed record WithdrawalError(string AccountNumber, string Message) : Response;
    }
}