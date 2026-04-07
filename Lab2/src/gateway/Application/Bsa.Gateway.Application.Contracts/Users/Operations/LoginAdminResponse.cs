using Bsa.Gateway.Application.Contracts.Users.Models;

namespace Bsa.Gateway.Application.Contracts.Users.Operations;

public abstract record LoginAdminResponse
{
    private LoginAdminResponse() { }

    public sealed record Success(SessionBaseDto AdminSession) : LoginAdminResponse;

    public sealed record Failure(string ErrorMessage) : LoginAdminResponse;
}