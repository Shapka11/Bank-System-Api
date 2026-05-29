using System.Diagnostics;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Activities;

public static class UserClientActivity
{
    public static string Name => "BankSystemApi.Gateway.Infrastructure.UserClient";

    public static ActivitySource ActivitySource { get; } = new(Name);
}