using Bsa.Application.Contracts.Models.Accounts;

namespace Bsa.Application.Contracts.Users.Operations;

public static class WithdrawUserAccount
{
    public readonly record struct Request(Guid Id, decimal Amount);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(AccountDto Account) : Response;

        public sealed record Failure(string Message) : Response;
    }
}