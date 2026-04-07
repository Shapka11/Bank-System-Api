using Bsa.Application.Contracts.Users.Models;

namespace Bsa.Application.Contracts.Users.Admin;

public static class LoginAdmin
{
    public readonly record struct Request(string Password);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(SessionBaseDto AdminSession) : Response;

        public sealed record InvalidPassword : Response;
    }
}