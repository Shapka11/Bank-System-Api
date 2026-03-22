namespace Bsa.Cli.Application.Abstractions.User.Operations;

public sealed class GetBalanceQuery
{
    public readonly record struct Request(Guid Id);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success(decimal Balance) : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}