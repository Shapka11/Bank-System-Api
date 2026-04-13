using Bsa.Application.Contracts.Accounts;
using Bsa.Application.Contracts.HistoryOperations;
using Bsa.Application.Contracts.Invoices;
using Bsa.Application.Contracts.Users;
using Bsa.Application.Options;
using Bsa.Application.Services;
using Bsa.Application.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace Bsa.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddOptions<SecurityOptions>()
            .BindConfiguration("Security")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        collection.AddScoped<IAdminService, AdminService>();
        collection.AddScoped<IUserService, UserService>();
        collection.AddScoped<IAccountService, AccountService>();
        collection.AddScoped<IHistoryOperationService, HistoryOperationService>();
        collection.AddScoped<IInvoiceService, InvoiceService>();

        collection.AddScoped<AccountSpecifications>();
        collection.AddScoped<InvoiceSpecifications>();
        collection.AddScoped<SessionSpecifications>();

        return collection;
    }
}