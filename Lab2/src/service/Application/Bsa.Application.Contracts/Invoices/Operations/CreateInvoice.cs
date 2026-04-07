using Bsa.Application.Contracts.Invoices.Models;

namespace Bsa.Application.Contracts.Invoices.Operations;

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

        public sealed record Success(InvoiceDto Invoice) : Response;

        public sealed record Unauthorized(Guid SessionId, string ErrorMessage) : Response;

        public record SenderAccountNotFound(string AccountNumber) : Response;

        public record ReceiverAccountNotFound(string AccountNumber) : Response;

        public record Forbidden(string Message) : Response;
    }
}