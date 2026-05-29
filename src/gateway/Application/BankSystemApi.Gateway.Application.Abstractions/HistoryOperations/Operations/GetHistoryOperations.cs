using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Models;

namespace BankSystemApi.Gateway.Application.Abstractions.HistoryOperations.Operations;

public static class GetHistoryOperations
{
    public readonly record struct Request(Guid UserId, long AccountId, int PageSize, string? PageToken);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(
            IReadOnlyCollection<BankHistoryOperationModel> History,
            string? PageToken) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}