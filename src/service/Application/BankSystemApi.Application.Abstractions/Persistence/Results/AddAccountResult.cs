using BankSystemApi.Domain.Accounts;

namespace BankSystemApi.Application.Abstractions.Persistence.Results;

public abstract record AddAccountResult
{
    private AddAccountResult() { }

    public sealed record Success(Account Account) : AddAccountResult;

    public sealed record AlreadyExist : AddAccountResult;
}