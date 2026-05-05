using System.Diagnostics;

namespace BankSystemApi.Application.Activities;

public static class InvoiceServiceActivity
{
    public static string Name => "BankSystemApi.Application.Services.InvoiceService";

    public static ActivitySource ActivitySource { get; } = new(Name);
}