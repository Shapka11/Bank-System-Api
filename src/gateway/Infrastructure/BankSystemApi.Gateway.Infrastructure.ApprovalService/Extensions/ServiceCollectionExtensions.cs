using BankSystemApi.ApprovalService.Invoices.Grpc;
using BankSystemApi.Gateway.Application.Abstractions.Invoices;
using BankSystemApi.Gateway.Infrastructure.ApprovalService.Clients;
using BankSystemApi.Gateway.Infrastructure.ApprovalService.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BankSystemApi.Gateway.Infrastructure.ApprovalService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureApprovalService(this IServiceCollection collection)
    {
        const string accountServiceName = "service-invoice-approval";

        collection.AddApprovalServiceOptions(accountServiceName);

        collection.AddApprovalSerivceGrpcClient<InvoiceService.InvoiceServiceClient>(accountServiceName);

        collection.AddScoped<IInvoiceApprovalClient, InvoiceApprovalClient>();

        return collection;
    }

    private static void AddApprovalServiceOptions(this IServiceCollection collection, string serviceName)
    {
        collection
            .AddOptions<ApprovalServiceOptions>(serviceName)
            .BindConfiguration($"Infrastructure:Clients:ApprovalServiceClients:{serviceName}")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static void AddApprovalSerivceGrpcClient<TClient>(this IServiceCollection collection, string serviceName)
        where TClient : class
    {
        collection.AddGrpcClient<TClient>(serviceName, (provider, options) =>
        {
            IOptionsMonitor<ApprovalServiceOptions> approvalServiceOptions =
                provider.GetRequiredService<IOptionsMonitor<ApprovalServiceOptions>>();
            options.Address = approvalServiceOptions.Get(serviceName).BaseAddress;
        });
    }
}