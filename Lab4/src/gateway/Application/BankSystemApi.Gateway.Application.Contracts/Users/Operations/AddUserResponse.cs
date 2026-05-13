namespace BankSystemApi.Gateway.Application.Contracts.Users.Operations;

public abstract record AddUserResponse
{
    private AddUserResponse() { }

    public sealed record Success(long UserId) : AddUserResponse;

    public sealed record Failure(string ErrorMessage) : AddUserResponse;
}