using Bsa.Cli.Application.Abstractions.User.Models;

namespace Bsa.Cli.Application.Abstractions.User.Operations;

public sealed class GetHistoryQuery
{
    public readonly record struct Request(Guid Id);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success(IEnumerable<AccountOperationEntity> History) : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}