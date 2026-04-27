using BankSystemApi.Application.Abstractions.Persistence;
using BankSystemApi.Application.Abstractions.Persistence.Repositories;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Builders;
using BankSystemApi.Infrastructure.Persistence.HistorySerializationChains.Factories;
using BankSystemApi.Infrastructure.Persistence.Plugins;
using BankSystemApi.Infrastructure.Persistence.Repositories;
using Itmo.Dev.Platform.Persistence.Abstractions.Extensions;
using Itmo.Dev.Platform.Persistence.Postgres.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace BankSystemApi.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection collection)
    {
        collection.AddPlatformPersistence(persistence => persistence
            .UsePostgres(postgres => postgres
                .WithConnectionOptions(builder => builder.BindConfiguration(
                    "Infrastructure:Persistence:Postgres"))
                .WithMigrationsFrom(typeof(IAssemblyMarker).Assembly)
                .WithDataSourcePlugin<InvoiceStatusMappingPlugin>()));

        collection.AddScoped<IPersistenceContext, PersistenceContext>();

        collection.AddScoped<IHistoryOperationRepository, HistoryOperationRepository>();
        collection.AddScoped<IAccountRepository, AccountRepository>();
        collection.AddScoped<IUserRepository, UserRepository>();
        collection.AddScoped<IInvoiceRepository, InvoiceRepository>();

        collection.AddScoped<IHistoryOperationSerializationChainBuilder, HistoryOperationSerializationChainBuilder>();
        collection.AddScoped<HistoryOperationSerializationChainFactory>();

        return collection;
    }
}