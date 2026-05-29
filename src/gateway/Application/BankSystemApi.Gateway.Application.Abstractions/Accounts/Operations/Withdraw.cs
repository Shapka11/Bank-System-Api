using BankSystemApi.Gateway.Application.Abstractions.Accounts.Models;

namespace BankSystemApi.Gateway.Application.Abstractions.Accounts.Operations;

public static class Withdraw
{
    public readonly record struct Request(Guid UserId, long AccountId, decimal Amount);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(BankAccountModel BankAccount) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}