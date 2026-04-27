using BankSystemApi.Gateway.Application.Abstractions.Accounts.Models;

namespace BankSystemApi.Gateway.Application.Abstractions.Accounts.Operations;

public static class GetAccounts
{
    public readonly record struct Request(Guid UserId, int PageSize, string? PageToken);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(
            IReadOnlyCollection<BankAccountModel> BankAccounts,
            string? PageToken) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}