using Bsa.Application.Contracts.Admins;
using Bsa.Application.Contracts.Users;
using Bsa.Application.Options;
using Bsa.Application.Providers;
using Bsa.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bsa.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection, IConfiguration configuration)
    {
        collection.AddOptions<SecurityOptions>()
            .BindConfiguration("Security")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        collection.AddScoped<IUserService, UserService>();
        collection.AddScoped<IAdminService, AdminService>();

        collection.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();

        return collection;
    }
}