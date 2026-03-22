using Bsa.Application.Abstractions.Persistence;
using Bsa.Application.Abstractions.Persistence.Repositories;
using Bsa.Infrastructure.Persistence.Options;
using Bsa.Infrastructure.Persistence.Repositories;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Bsa.Infrastructure.Persistence;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddPersistence(this IServiceCollection collection, IConfiguration configuration)
    {
        IConfigurationSection postgresConfig = configuration.GetSection("Infrastructure:Persistence:Postgres");
        collection.Configure<PostresOptions>(postgresConfig);

        PostresOptions options = postgresConfig.Get<PostresOptions>()
                ?? throw new InvalidOperationException("Postgres configuration is missing");
        string connectionString = options.ToConnectionString();

        collection.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));

        collection.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(IAssemblyMarker).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        collection.AddScoped<IPersistenceContext, PersistenceContext>();

        collection.AddScoped<IAccountOperationRepository, AccountOperationRepository>();
        collection.AddScoped<IAccountRepository, AccountRepository>();
        collection.AddScoped<IAdminSessionRepository, AdminSessionRepository>();
        collection.AddScoped<IUserSessionRepository, UserSessionRepository>();
        collection.AddScoped<IAccountOperationRepository, AccountOperationRepository>();

        return collection;
    }
}