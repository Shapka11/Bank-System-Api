namespace Bsa.Application.Contracts.Users.User;

public static class LogoutUser
{
    public readonly record struct Request(Guid Id);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success() : Response;

        public sealed record Unauthorized(Guid SessionId, string ErrorMessage) : Response;
    }
}