using Bsa.Gateway.Application.Abstractions.Users.Models;

namespace Bsa.Gateway.Application.Abstractions.Users.User;

public static class LoginUser
{
    public readonly record struct Request(string AccountNumber, string Password);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(BankSessionBaseModel BankUserSession) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}