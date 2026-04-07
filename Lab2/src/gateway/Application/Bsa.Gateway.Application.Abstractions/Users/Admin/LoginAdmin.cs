using Bsa.Gateway.Application.Abstractions.Users.Models;

namespace Bsa.Gateway.Application.Abstractions.Users.Admin;

public static class LoginAdmin
{
    public readonly record struct Request(string Password);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(BankSessionBaseModel BankAdminSession) : Response;

        public sealed record Failure(string ErrorMessage) : Response;
    }
}