using BankSystemApi.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Application.Contracts.Invoices.Operations;

public static class CreateInvoice
{
    public readonly record struct Request(
        Guid UserId,
        Guid SenderAccountId,
        Guid ReceiverAccountId,
        decimal Amount);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(InvoiceDto Invoice) : Response;

        public sealed record Unauthorized(Guid UserId) : Response;

        public record SenderAccountNotFound(Guid AccountId) : Response;

        public record ReceiverAccountNotFound(Guid AccountId) : Response;

        public record Forbidden(string Message) : Response;
    }
}