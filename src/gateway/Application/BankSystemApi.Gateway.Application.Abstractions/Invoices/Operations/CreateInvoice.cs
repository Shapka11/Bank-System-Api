using BankSystemApi.Gateway.Application.Abstractions.Invoices.Models;

namespace BankSystemApi.Gateway.Application.Abstractions.Invoices.Operations;

public static class CreateInvoice
{
    public readonly record struct Request(
        Guid UserId,
        long SenderAccountId,
        long ReceiverAccountId,
        decimal Amount);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(BankInvoiceModel BankInvoice) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}