using Bsa.Cli.Application.Abstractions.Admin;
using Bsa.Cli.Infrastructure.ClientService.Admins.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace Bsa.Cli.Infrastructure.ClientService.Admins;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddAdminClient(this IServiceCollection collection)
    {
        collection
            .AddOptions<AdminClientOptions>()
            .BindConfiguration("Infrastructure:Configuration")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        collection
            .AddRefitClient<IRefitAdminClient>()
            .ConfigureHttpClient((provider, client) =>
            {
                IOptions<AdminClientOptions> options = provider.GetRequiredService<IOptions<AdminClientOptions>>();
                client.BaseAddress = options.Value.Address;
            });

        collection.AddScoped<IAdminClient, AdminClient>();

        return collection;
    }
}