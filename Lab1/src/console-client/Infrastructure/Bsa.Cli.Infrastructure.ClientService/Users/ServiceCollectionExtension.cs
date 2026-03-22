using Bsa.Cli.Application.Abstractions.User;
using Bsa.Cli.Infrastructure.ClientService.Users.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace Bsa.Cli.Infrastructure.ClientService.Users;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddUserClient(this IServiceCollection collection)
    {
        collection
            .AddOptions<UserClientOptions>()
            .BindConfiguration("Infrastructure:Configuration")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        collection
            .AddRefitClient<IRefitUserClient>()
            .ConfigureHttpClient((provider, client) =>
            {
                IOptions<UserClientOptions> options = provider.GetRequiredService<IOptions<UserClientOptions>>();
                client.BaseAddress = options.Value.Address;
            });

        collection.AddScoped<IUserClient, UserClient>();

        return collection;
    }
}