namespace Bsa.Cli.Application.Contracts.User.Operations;

public sealed class Deposit
{
    public readonly record struct Request(decimal Amount);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success() : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}