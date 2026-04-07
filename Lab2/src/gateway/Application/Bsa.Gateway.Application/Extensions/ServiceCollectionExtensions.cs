using Bsa.Gateway.Application.Contracts.Accounts;
using Bsa.Gateway.Application.Contracts.HistoryOperations;
using Bsa.Gateway.Application.Contracts.Invoices;
using Bsa.Gateway.Application.Contracts.Users;
using Bsa.Gateway.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bsa.Gateway.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
       collection.AddScoped<IAccountService, AccountService>();
       collection.AddScoped<IInvoiceService, InvoiceService>();
       collection.AddScoped<IUserService, UserService>();
       collection.AddScoped<IAdminService, AdminService>();
       collection.AddScoped<IHistoryOperationService, HistoryOperationService>();

       return collection;
    }
}