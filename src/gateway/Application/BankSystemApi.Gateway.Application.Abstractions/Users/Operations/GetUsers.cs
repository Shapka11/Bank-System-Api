using BankSystemApi.Gateway.Application.Abstractions.Users.Models;

namespace BankSystemApi.Gateway.Application.Abstractions.Users.Operations;

public static class GetUsers
{
    public readonly record struct Request(
        IEnumerable<Guid> AuthorizationIds,
        IEnumerable<long> UserIds,
        int PageSize);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(IReadOnlyCollection<BankUserModel> Users) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}