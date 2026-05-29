using BankSystemApi.Accounts.Grpc;
using BankSystemApi.Gateway.Application.Abstractions.Accounts;
using BankSystemApi.Gateway.Application.Abstractions.HistoryOperations;
using BankSystemApi.Gateway.Application.Abstractions.Invoices;
using BankSystemApi.Gateway.Application.Abstractions.Users;
using BankSystemApi.Gateway.Infrastructure.BankService.Clients;
using BankSystemApi.Gateway.Infrastructure.BankService.Options;
using BankSystemApi.HistoryOperations.Grpc;
using BankSystemApi.Invoices.Grpc;
using BankSystemApi.Users.Grpc;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureBankService(this IServiceCollection collection)
    {
        const string accountServiceName = "service-account";
        const string invoiceServiceName = "service-invoice";
        const string userServiceName = "service-user";
        const string historyServiceName = "service-history";

        collection.AddBankServiceOptions(accountServiceName);
        collection.AddBankServiceOptions(invoiceServiceName);
        collection.AddBankServiceOptions(userServiceName);
        collection.AddBankServiceOptions(historyServiceName);

        collection.AddBankGrpcClient<AccountService.AccountServiceClient>(accountServiceName);
        collection.AddBankGrpcClient<InvoiceService.InvoiceServiceClient>(invoiceServiceName);
        collection.AddBankGrpcClient<UserService.UserServiceClient>(userServiceName);
        collection.AddBankGrpcClient<HistoryOperationService.HistoryOperationServiceClient>(historyServiceName);

        collection.AddScoped<IAccountClient, AccountClient>();
        collection.AddScoped<IInvoiceClient, InvoiceClient>();
        collection.AddScoped<IUserClient, UserClient>();
        collection.AddScoped<IHistoryOperationClient, HistoryOperationClient>();

        return collection;
    }

    private static void AddBankServiceOptions(this IServiceCollection collection, string serviceName)
    {
        collection
            .AddOptions<BankServiceOptions>(serviceName)
            .BindConfiguration($"Infrastructure:Clients:BankServiceClients:{serviceName}")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static void AddBankGrpcClient<TClient>(this IServiceCollection collection, string serviceName)
        where TClient : ClientBase<TClient>
    {
        collection.AddGrpcClient<TClient>((provider, options) =>
        {
            IOptionsMonitor<BankServiceOptions> bankOptions =
                provider.GetRequiredService<IOptionsMonitor<BankServiceOptions>>();
            options.Address = bankOptions.Get(serviceName).BaseAddress;
        });
    }
}