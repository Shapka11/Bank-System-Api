namespace Bsa.Cli.Application.Abstractions.Admin.Operations;

public sealed class CreateAccountQuery
{
    public readonly record struct Request(Guid Id, string AccountNumber, string Password);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success() : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}