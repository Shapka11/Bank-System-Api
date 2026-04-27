namespace BankSystemApi.Application.Contracts.Users.Operations;

public static class AddUser
{
    public readonly record struct Request(Guid AuthorizationId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success() : Response;
    }
}