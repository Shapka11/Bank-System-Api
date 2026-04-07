namespace Bsa.Gateway.Application.Contracts.Users.Operations;

public abstract record LogoutUserResponse
{
    private LogoutUserResponse() { }

    public sealed record Success() : LogoutUserResponse;

    public sealed record Failure(string ErrorMessage) : LogoutUserResponse;
}