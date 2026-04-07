using Bsa.Gateway.Application.Abstractions.Invoices.Models;

namespace Bsa.Gateway.Application.Abstractions.Invoices.Operations;

public static class GetOutgoingInvoices
{
    public readonly record struct Request(
        Guid SessionId,
        IEnumerable<string> ReceiverAccountNumbers,
        IEnumerable<BankInvoiceStatusModel> Statuses,
        int PageSize,
        string? PageToken);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(IEnumerable<BankInvoiceModel> Invoices, string? PageToken) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}