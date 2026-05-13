using BankSystemApi.Application.Contracts.HistoryOperations.Models;

namespace BankSystemApi.Application.Contracts.HistoryOperations.Operations;

public static class GetHistoryOperations
{
    public readonly record struct PageToken(long Id);

    public readonly record struct Request(Guid UserId, Guid AccountId, int PageSize, PageToken? PageToken);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(IReadOnlyCollection<HistoryOperationDto> History, PageToken? PageToken) : Response;

        public sealed record Unauthorized(Guid UserId) : Response;

        public sealed record AccountNotFound(Guid AccountId) : Response;

        public sealed record Forbidden(string ErrorMessage) : Response;
    }
}