using BankSystemApi.Application.Contracts.Accounts.Models;

namespace BankSystemApi.Application.Contracts.Accounts.Operations;

public static class CreateAccount
{
    public readonly record struct Request(
        Guid CallerUserId,
        long TargetUserId,
        string AccountNumber,
        string Password,
        AccountTypeDto AccountType);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(AccountDto Account) : Response;

        public sealed record Unauthorized(string UserId) : Response;

        public sealed record AccountAlreadyExists(string AccountNumber) : Response;

        public sealed record ReachedAccountLimit(string ErrorMessage) : Response;
    }
}