namespace Bsa.Cli.Application.Abstractions.User.Operations;

public sealed class LoginUserQuery
{
    public readonly record struct Request(string AccountNumber, string Password);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success(Guid SessionId) : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}