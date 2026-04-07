using Bsa.Application.Contracts.Accounts.Models;

namespace Bsa.Application.Contracts.Accounts.Operations;

public static class CreateAccount
{
    public readonly record struct Request(Guid Id, string AccountNumber, string Password);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(AccountDto Account) : Response;

        public sealed record Unauthorized(Guid SessionId, string ErrorMessage) : Response;

        public sealed record AccountAlreadyExists(string AccountNumber) : Response;
    }
}