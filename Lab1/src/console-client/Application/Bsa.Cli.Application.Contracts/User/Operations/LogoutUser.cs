namespace Bsa.Cli.Application.Contracts.User.Operations;

public sealed class LogoutUser
{
    public abstract record Result
    {
        private Result() { }

        public sealed record Success() : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}