using BankSystemApi.Application.Contracts.Accounts.Models;

namespace BankSystemApi.Application.Contracts.Accounts.Operations;

public static class GetAccounts
{
    public readonly record struct PageToken(long Id);

    public readonly record struct Request(Guid UserId, int PageSize, PageToken? PageToken);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(IReadOnlyCollection<AccountDto> Accounts, PageToken? PageToken) : Response;

        public sealed record Unauthorized(Guid UserId) : Response;
    }
}