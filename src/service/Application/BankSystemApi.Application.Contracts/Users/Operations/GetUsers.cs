using BankSystemApi.Application.Contracts.Users.Models;

namespace BankSystemApi.Application.Contracts.Users.Operations;

public static class GetUsers
{
    public readonly record struct Request(
        IEnumerable<Guid> AuthorizationIds,
        IEnumerable<long> UserIds,
        int PageSize);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(IReadOnlyCollection<UserDto> Users) : Response;
    }
}