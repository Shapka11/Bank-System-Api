using Bsa.Gateway.Application.Contracts.Users.Models;

namespace Bsa.Gateway.Application.Contracts.Users.Operations;

public abstract record LoginUserResponse
{
    private LoginUserResponse() { }

    public sealed record Success(SessionBaseDto UserSession) : LoginUserResponse;

    public sealed record Failure(string ErrorMessage) : LoginUserResponse;
}