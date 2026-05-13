using System.Diagnostics;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Activities;

public static class InvoiceClientActivity
{
    public static string Name => "BankSystemApi.Gateway.Infrastructure.InvoiceClient";

    public static ActivitySource ActivitySource { get; } = new(Name);
}