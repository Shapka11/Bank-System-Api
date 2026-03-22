namespace Bsa.Cli.Application.Contracts.Admin.Operations;

public sealed class CreateAccount
{
    public readonly record struct Request(string AccountNumber, string Password);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success() : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}