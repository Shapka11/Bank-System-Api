using Bsa.Application.Contracts.Models.Sessions;

namespace Bsa.Application.Contracts.Users.Operations;

public static class LoginUser
{
    public readonly record struct Request(string AccountNumber, string Password);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(UserSessionDto UserSession) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}