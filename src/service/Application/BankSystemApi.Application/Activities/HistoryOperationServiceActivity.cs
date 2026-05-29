using System.Diagnostics;

namespace BankSystemApi.Application.Activities;

public static class HistoryOperationServiceActivity
{
    public static string Name => "BankSystemApi.Application.Services.HistoryOperationService";

    public static ActivitySource ActivitySource { get; } = new(Name);
}