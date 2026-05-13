using System.Diagnostics;

namespace BankSystemApi.Application.Activities;

public static class UserServiceActivity
{
    public static string Name => "BankSystemApi.Application.Services.UserService";

    public static ActivitySource ActivitySource { get; } = new(Name);
}