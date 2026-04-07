using Bsa.Gateway.Application.Abstractions.Accounts.Models;

namespace Bsa.Gateway.Application.Abstractions.Accounts.Operations;

public static class CreateAccount
{
    public readonly record struct Request(Guid Id, string AccountNumber, string Password);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(BankAccountModel BankAccount) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}