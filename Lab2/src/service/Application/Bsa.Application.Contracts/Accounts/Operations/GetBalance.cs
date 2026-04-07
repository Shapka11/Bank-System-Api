namespace Bsa.Application.Contracts.Accounts.Operations;

public static class GetBalance
{
    public readonly record struct Request(Guid Id);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(decimal Balance) : Response;

        public sealed record Unauthorized(Guid SessionId, string ErrorMessage) : Response;

        public sealed record AccountNotFound(long AccountId) : Response;
    }
}