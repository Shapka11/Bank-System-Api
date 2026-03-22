using Bsa.Application.Contracts.Models.Operations;

namespace Bsa.Application.Contracts.Users.Operations;

public static class GetUserOperationHistory
{
    public readonly record struct PageToken(long Id);

    public readonly record struct Request(Guid Id, int PageSize, PageToken? PageToken);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(AccountOperationDto[] History, PageToken? PageToken) : Response;

        public sealed record Failure(string Message) : Response;
    }
}