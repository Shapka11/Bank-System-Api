namespace BankSystemApi.Gateway.Application.Abstractions.Invoices.Operations;

public static class ApproveInvoice
{
    public readonly record struct Request(long UserId, long InvoiceId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success() : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}