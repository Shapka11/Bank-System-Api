using BankSystemApi.Gateway.Application.Abstractions.Invoices.Models;

namespace BankSystemApi.Gateway.Application.Abstractions.Invoices.Operations;

public static class GetOutgoingInvoices
{
    public readonly record struct Request(
        Guid UserId,
        IEnumerable<long> ReceiverAccountIds,
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