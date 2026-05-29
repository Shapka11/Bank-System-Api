using System.Diagnostics;

namespace BankSystemApi.Gateway.Infrastructure.ApprovalService.Extensions;

public static class ActivityExtensions
{
    public static void AddUserIdBaggage(this Activity? activity, long userId)
    {
        activity?.AddTag("user.id", userId);
        activity?.AddBaggage("user.id", userId.ToString());
    }

    public static void AddInvoiceIdBaggage(this Activity? activity, long invoiceId)
    {
        activity?.AddTag("invoice.id", invoiceId);
        activity?.AddBaggage("invoice.id", invoiceId.ToString());
    }
}