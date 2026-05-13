using BankSystemApi.Application.Contracts.Accounts;
using BankSystemApi.Application.Contracts.HistoryOperations;
using BankSystemApi.Application.Contracts.Invoices;
using BankSystemApi.Application.Contracts.Users;
using BankSystemApi.Application.Options;
using BankSystemApi.Application.Providers;
using BankSystemApi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BankSystemApi.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddOptions<AccountOptions>()
            .BindConfiguration("Account")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        collection.AddScoped<IUserService, UserService>();
        collection.AddScoped<IAccountService, AccountService>();
        collection.AddScoped<IHistoryOperationService, HistoryOperationService>();
        collection.AddScoped<IInvoiceService, InvoiceService>();

        collection.AddScoped<IGuidProvider, GuidProvider>();

        return collection;
    }
}