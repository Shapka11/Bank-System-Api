using Bsa.Application.Contracts.Models.Sessions;

namespace Bsa.Application.Contracts.Admins.Operations;

public static class LoginAdmin
{
    public readonly record struct Request(string Password);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(AdminSessionDto AdminSession) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}