using Bsa.Cli.Application.Contracts.User.Models;

namespace Bsa.Cli.Application.Contracts.User.Operations;

public sealed class GetHistory
{
    public abstract record Result
    {
        private Result() { }

        public sealed record Success(IEnumerable<AccountOperationDto> History) : Result;

        public sealed record Failure(string ErrorMessage) : Result;
    }
}