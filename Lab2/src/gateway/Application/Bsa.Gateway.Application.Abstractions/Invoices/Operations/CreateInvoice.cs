using Bsa.Gateway.Application.Abstractions.Invoices.Models;

namespace Bsa.Gateway.Application.Abstractions.Invoices.Operations;

public static class CreateInvoice
{
    public readonly record struct Request(
        Guid SessionId,
        string SenderAccountNumber,
        string ReceiverAccountNumber,
        decimal Amount);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(BankInvoiceModel BankInvoice) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}