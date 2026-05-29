namespace BankSystemApi.Gateway.Application.Abstractions.Invoices.Operations;

public static class AssignAccountant
{
    public readonly record struct Request(long InvoiceId, long UserId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success() : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}