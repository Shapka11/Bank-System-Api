namespace Bsa.Cli.Application.Abstractions.Admin.Operations;

public sealed class LoginAdminQuery
{
    public readonly record struct Request(string Password);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success(Guid SessionId) : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}