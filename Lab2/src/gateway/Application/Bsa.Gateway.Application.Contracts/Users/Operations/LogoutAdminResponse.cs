namespace Bsa.Gateway.Application.Contracts.Users.Operations;

public abstract record LogoutAdminResponse
{
    private LogoutAdminResponse() { }

    public sealed record Success() : LogoutAdminResponse;

    public sealed record Failure(string ErrorMessage) : LogoutAdminResponse;
}