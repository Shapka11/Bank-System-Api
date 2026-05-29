using BankSystemApi.Application.Contracts.Accounts.Models;

namespace BankSystemApi.Application.Contracts.Accounts.Operations;

public static class Deposit
{
    public readonly record struct Request(Guid UserId, long AccountId, decimal Amount);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(AccountDto Account) : Response;

        public sealed record Unauthorized(Guid UserId) : Response;

        public sealed record AccountNotFound(long AccountId) : Response;

        public sealed record Forbidden(string ErrorMessage) : Response;
    }
}