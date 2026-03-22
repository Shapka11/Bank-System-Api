namespace Bsa.Cli.Application.Contracts.User.Operations;

public sealed class GetBalance
{
    public abstract record Result
    {
        private Result() { }

        public sealed record Success(decimal Balance) : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}