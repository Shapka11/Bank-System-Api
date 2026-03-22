using Bsa.Application.Contracts.Models.Accounts;

namespace Bsa.Application.Contracts.Admins.Operations;

public static class CreateAccount
{
    public readonly record struct Request(Guid Id, string AccountNumber, string Password);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(AccountDto Account) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}