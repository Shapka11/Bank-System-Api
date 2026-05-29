namespace BankSystemApi.Application.Contracts.Accounts.Operations;

public static class GetBalance
{
    public readonly record struct Request(Guid UserId, long AccountId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(decimal Balance) : Response;

        public sealed record Unauthorized(Guid UserId) : Response;

        public sealed record AccountNotFound(long AccountId) : Response;

        public sealed record Forbidden(string ErrorMessage) : Response;
    }
}