using Bsa.Application.Contracts.HistoryOperations.Models;

namespace Bsa.Application.Contracts.HistoryOperations.Operations;

public static class GetHistoryOperations
{
    public readonly record struct PageToken(long Id);

    public readonly record struct Request(Guid Id, int PageSize, PageToken? PageToken);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(IEnumerable<HistoryOperationDto> History, PageToken? PageToken) : Response;

        public sealed record Unauthorized(Guid SessionId, string ErrorMessage) : Response;
    }
}