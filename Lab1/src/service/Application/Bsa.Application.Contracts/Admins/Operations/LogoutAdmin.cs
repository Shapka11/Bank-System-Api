namespace Bsa.Application.Contracts.Admins.Operations;

public static class LogoutAdmin
{
    public readonly record struct Request(Guid Id);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success() : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}