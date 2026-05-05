using System.Diagnostics;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Activities;

public static class AccountClientActivity
{
    public static string Name => "BankSystemApi.Gateway.Infrastructure.AccountClient";

    public static ActivitySource ActivitySource { get; } = new(Name);
}