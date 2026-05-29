using BankSystemApi.Gateway.Application.Abstractions.Users.Models;

namespace BankSystemApi.Gateway.Application.Abstractions.Users.Operations;

public static class AddUser
{
    public readonly record struct Request(Guid AuthorizationId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(BankUserModel User) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}