using BankSystemApi.Domain.Users;

namespace BankSystemApi.Application.Abstractions.Persistence.Results;

public abstract record AddUserResult
{
    private AddUserResult() { }

    public sealed record Success(User User) : AddUserResult;

    public sealed record AlreadyExist : AddUserResult;
}