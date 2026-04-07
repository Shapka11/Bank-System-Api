using Bsa.Gateway.Application.Abstractions.Invoices.Models;

namespace Bsa.Gateway.Application.Abstractions.Invoices.Operations;

public static class PayInvoice
{
    public readonly record struct Request(Guid SessionId, long InvoiceId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(BankInvoiceModel BankInvoice) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}