namespace Bsa.Gateway.Application.Abstractions.Accounts.Operations;

public static class GetBalance
{
    public readonly record struct Request(Guid Id);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(decimal Money) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}