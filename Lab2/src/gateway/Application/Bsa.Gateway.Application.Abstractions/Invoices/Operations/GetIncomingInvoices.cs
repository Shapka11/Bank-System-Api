using Bsa.Gateway.Application.Abstractions.Invoices.Models;

namespace Bsa.Gateway.Application.Abstractions.Invoices.Operations;

public static class GetIncomingInvoices
{
    public readonly record struct Request(
        Guid SessionId,
        IEnumerable<string> SenderAccountNumbers,
        IEnumerable<BankInvoiceStatusModel> Statuses,
        int PageSize,
        string? PageToken);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(IReadOnlyCollection<BankInvoiceModel> Invoices, string? PageToken) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}