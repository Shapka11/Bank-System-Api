using Bsa.Cli.Presentation.Cli.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Bsa.Cli.Presentation.Cli;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddPresentationConsole(this IServiceCollection collection)
    {
        collection
            .AddOptions<CliOptions>()
            .BindConfiguration("Presentation:Cli")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return collection;
    }
}