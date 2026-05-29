using System.Diagnostics;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Extensions;

public static class ActivityExtensions
{
    public static void AddUserIdBaggage(this Activity? activity, Guid userId)
    {
        activity?.AddTag("user.id", userId);
        activity?.AddBaggage("user.id", userId.ToString());
    }

    public static void AddAccountIdBaggage(this Activity? activity, long accountId)
    {
        activity?.AddTag("account.id", accountId.ToString());
        activity?.AddBaggage("account.id", accountId.ToString());
    }

    public static void AddInvoiceIdBaggage(this Activity? activity, long invoiceId)
    {
        activity?.AddTag("invoice.id", invoiceId);
        activity?.AddBaggage("invoice.id", invoiceId.ToString());
    }
}