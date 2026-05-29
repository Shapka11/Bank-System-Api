using BankSystemApi.Gateway.Application.Abstractions.Accounts.Models;

namespace BankSystemApi.Gateway.Application.Abstractions.Accounts.Operations;

public static class CreateAccount
{
    public readonly record struct Request(
        Guid CallerUserId,
        long TargetUserId,
        string AccountNumber,
        string Password,
        BankAccountTypeModel AccountType);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(BankAccountModel BankAccount) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}