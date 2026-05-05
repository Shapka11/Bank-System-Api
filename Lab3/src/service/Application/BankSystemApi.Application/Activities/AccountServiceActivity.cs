using System.Diagnostics;

namespace BankSystemApi.Application.Activities;

public static class AccountServiceActivity
{
    public static string Name => "BankSystemApi.Application.Services.AccountService";

    public static ActivitySource ActivitySource { get; } = new(Name);
}