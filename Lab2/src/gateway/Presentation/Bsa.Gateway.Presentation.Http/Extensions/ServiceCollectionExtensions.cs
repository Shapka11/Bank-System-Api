namespace Bsa.Gateway.Presentation.Http.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationHttp(this IServiceCollection collection)
    {
        collection.AddControllers();
        return collection;
    }
}