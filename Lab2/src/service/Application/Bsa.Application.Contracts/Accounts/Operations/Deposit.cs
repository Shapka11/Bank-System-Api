using Bsa.Application.Contracts.Accounts.Models;

namespace Bsa.Application.Contracts.Accounts.Operations;

public static class Deposit
{
    public readonly record struct Request(Guid Id, decimal Amount);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(AccountDto Account) : Response;

        public sealed record Unauthorized(Guid SessionId, string ErrorMessage) : Response;

        public sealed record AccountNotFound(long AccountId) : Response;
    }
}