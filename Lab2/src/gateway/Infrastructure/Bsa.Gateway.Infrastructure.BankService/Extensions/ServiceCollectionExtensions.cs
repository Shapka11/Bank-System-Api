using Bsa.CsharpBackend.Grpc;
using Bsa.Gateway.Application.Abstractions.Accounts;
using Bsa.Gateway.Application.Abstractions.HistoryOperations;
using Bsa.Gateway.Application.Abstractions.Invoices;
using Bsa.Gateway.Application.Abstractions.Users;
using Bsa.Gateway.Infrastructure.BankService.Clients;
using Bsa.Gateway.Infrastructure.BankService.Options;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bsa.Gateway.Infrastructure.BankService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection collection)
    {
        collection
            .AddOptions<BankServiceOptions>()
            .BindConfiguration("Infrastructure:Clients:BankServiceClient")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        collection.AddBankGrpcClient<AccountService.AccountServiceClient>();
        collection.AddBankGrpcClient<InvoiceService.InvoiceServiceClient>();
        collection.AddBankGrpcClient<AdminService.AdminServiceClient>();
        collection.AddBankGrpcClient<UserService.UserServiceClient>();
        collection.AddBankGrpcClient<HistoryOperationService.HistoryOperationServiceClient>();

        collection.AddScoped<IAccountClient, AccountClient>();
        collection.AddScoped<IInvoiceClient, InvoiceClient>();
        collection.AddScoped<IAdminClient, AdminClient>();
        collection.AddScoped<IUserClient, UserClient>();
        collection.AddScoped<IHistoryOperationClient, HistoryOperationClient>();

        return collection;
    }

    private static void AddBankGrpcClient<TClient>(this IServiceCollection collection)
        where TClient : ClientBase<TClient>
    {
        collection.AddGrpcClient<TClient>((provider, options) =>
        {
            BankServiceOptions bankOptions = provider.GetRequiredService<IOptions<BankServiceOptions>>().Value;
            options.Address = bankOptions.BaseAddress;
        });
    }
}