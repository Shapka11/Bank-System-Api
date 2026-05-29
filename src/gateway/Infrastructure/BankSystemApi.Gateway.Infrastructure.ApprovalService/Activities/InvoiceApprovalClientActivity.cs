using System.Diagnostics;

namespace BankSystemApi.Gateway.Infrastructure.ApprovalService.Activities;

public static class InvoiceApprovalClientActivity
{
    public static string Name => "BankSystemApi.Gateway.Infrastructure.InvoiceApprovalClient";

    public static ActivitySource ActivitySource { get; } = new(Name);
}