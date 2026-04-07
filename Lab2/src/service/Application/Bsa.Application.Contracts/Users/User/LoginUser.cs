using Bsa.Application.Contracts.Users.Models;

namespace Bsa.Application.Contracts.Users.User;

public static class LoginUser
{
    public readonly record struct Request(string AccountNumber, string Password);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(SessionBaseDto UserSession) : Response;

        public sealed record InvalidPassword : Response;

        public sealed record AccountNotFound(string AccountNumber) : Response;
    }
}