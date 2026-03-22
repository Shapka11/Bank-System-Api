using Bsa.Cli.Application.Contracts.Admin;
using Bsa.Cli.Application.Contracts.User;
using Bsa.Cli.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bsa.Cli.Application;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<IAdminService, AdminService>();
        collection.AddScoped<IUserService, UserService>();

        return collection;
    }
}