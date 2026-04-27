using System.Diagnostics;

namespace BankSystemApi.Gateway.Presentation.Http.Extensions;

public static class ActivityExtensions
{
    public static void AddUserIdBaggage(this Activity? activity, string userId)
    {
        activity?.AddTag("user.id", userId);
        activity?.AddBaggage("user.id", userId);
    }

    public static void AddAccountIdBaggage(this Activity? activity, Guid accountId)
    {
        activity?.AddTag("account.id", accountId.ToString());
        activity?.AddBaggage("account.id", accountId.ToString());
    }
}