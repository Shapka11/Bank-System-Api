using System.Diagnostics;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Activities;

public static class HistoryOperationClientActivity
{
    public static string Name => "BankSystemApi.Gateway.Infrastructure.HistoryOperationClient";

    public static ActivitySource ActivitySource { get; } = new(Name);
}