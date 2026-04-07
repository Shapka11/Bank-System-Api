namespace Bsa.Application.Contracts.Users.Admin;

public static class LogoutAdmin
{
    public readonly record struct Request(Guid Id);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success() : Response;

        public sealed record Unauthorized(Guid SessionId, string ErrorMessage) : Response;
    }
}