namespace Bsa.Cli.Application.Abstractions.User.Operations;

public sealed class DepositQuery
{
    public readonly record struct Request(Guid Id, decimal Amount);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success() : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}