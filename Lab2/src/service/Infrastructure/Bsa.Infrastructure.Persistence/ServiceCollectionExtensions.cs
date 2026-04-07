using Bsa.Application.Abstractions.Persistence;
using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Builders;
using Bsa.Infrastructure.Persistence.HistorySerializationChains.Factories;
using Bsa.Infrastructure.Persistence.Plugins;
using Bsa.Infrastructure.Persistence.Repositories;
using Itmo.Dev.Platform.Persistence.Abstractions.Extensions;
using Itmo.Dev.Platform.Persistence.Postgres.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Bsa.Infrastructure.Persistence;

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
        collection.AddScoped<ISessionRepository, SessionRepository>();
        collection.AddScoped<IInvoiceRepository, InvoiceRepository>();

        collection.AddScoped<IHistoryOperationSerializationChainBuilder, HistoryOperationSerializationChainBuilder>();
        collection.AddScoped<HistoryOperationSerializationChainFactory>();

        return collection;
    }
}