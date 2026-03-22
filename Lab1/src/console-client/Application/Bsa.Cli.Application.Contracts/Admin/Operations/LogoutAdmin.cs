namespace Bsa.Cli.Application.Contracts.Admin.Operations;

public sealed class LogoutAdmin
{
    public abstract record Result
    {
        private Result() { }

        public sealed record Success() : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}