namespace Bsa.Cli.Application.Contracts.User.Operations;

public sealed class LoginUser
{
    public readonly record struct Request(string AccountNumber, string Password);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success() : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}