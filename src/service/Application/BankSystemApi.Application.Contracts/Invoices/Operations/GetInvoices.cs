using BankSystemApi.Application.Contracts.Invoices.Models;

namespace BankSystemApi.Application.Contracts.Invoices.Operations;

public static class GetInvoices
{
    public readonly record struct PageToken(long Id);

    public readonly record struct Request(
        Guid UserId,
        IEnumerable<long> OtherUserAccountIds,
        IEnumerable<InvoiceStatusDto> Statuses,
        InvoiceTypeDto InvoiceType,
        int PageSize,
        PageToken? PageToken);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(IReadOnlyCollection<InvoiceDto> Invoices, PageToken? PageToken) : Response;

        public sealed record Unauthorized(Guid UserId) : Response;
    }
}