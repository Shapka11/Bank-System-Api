namespace BankSystemApi.Gateway.Application.Abstractions.Users.Operations;

public static class AddUser
{
    public readonly record struct Request(Guid AuthorizationId);

    public abstract record Response
    {
        protected Response() { }

        public sealed record Success() : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}