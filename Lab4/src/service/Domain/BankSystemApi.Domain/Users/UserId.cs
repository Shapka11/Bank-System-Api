namespace BankSystemApi.Domain.Users;

public readonly record struct UserId(long Value)
{
    public static UserId Default => new(default);
}