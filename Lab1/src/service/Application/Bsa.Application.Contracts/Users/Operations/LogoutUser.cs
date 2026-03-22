namespace Bsa.Application.Contracts.Users.Operations;

public static class LogoutUser
{
    public readonly record struct Request(Guid Id);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success() : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}