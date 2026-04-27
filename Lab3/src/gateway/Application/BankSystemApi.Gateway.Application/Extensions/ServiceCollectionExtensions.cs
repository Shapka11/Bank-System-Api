using BankSystemApi.Gateway.Application.Contracts.Accounts;
using BankSystemApi.Gateway.Application.Contracts.HistoryOperations;
using BankSystemApi.Gateway.Application.Contracts.Invoices;
using BankSystemApi.Gateway.Application.Contracts.Users;
using BankSystemApi.Gateway.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BankSystemApi.Gateway.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
       collection.AddScoped<IAccountService, AccountService>();
       collection.AddScoped<IInvoiceService, InvoiceService>();
       collection.AddScoped<IUserService, UserService>();
       collection.AddScoped<IHistoryOperationService, HistoryOperationService>();

       return collection;
    }
}