using Bsa.Application.Contracts.Invoices.Models;

namespace Bsa.Application.Contracts.Invoices.Operations;

public static class GetIncomingInvoices
{
    public readonly record struct PageToken(long Id);

    public readonly record struct Request(
        Guid SessionId,
        IEnumerable<string> SenderAccountNumbers,
        IEnumerable<InvoiceStatusDto> Statuses,
        int PageSize,
        PageToken? PageToken);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(IEnumerable<InvoiceDto> Invoices, PageToken? PageToken) : Response;

        public sealed record Unauthorized(Guid SessionId, string ErrorMessage) : Response;

        public sealed record InvalidStatus(string Message) : Response;

        public sealed record AccountNotFound(string Message) : Response;
    }
}