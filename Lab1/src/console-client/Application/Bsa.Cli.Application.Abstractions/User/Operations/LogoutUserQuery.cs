namespace Bsa.Cli.Application.Abstractions.User.Operations;

public sealed class LogoutUserQuery
{
    public readonly record struct Request(Guid SessionId);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success() : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}