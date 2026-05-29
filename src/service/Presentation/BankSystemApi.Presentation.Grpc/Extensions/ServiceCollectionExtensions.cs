using BankSystemApi.Presentation.Grpc.Interceptors;

namespace BankSystemApi.Presentation.Grpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationGrpc(this IServiceCollection collection)
    {
        collection.AddGrpc(options => options.Interceptors.Add<ValidationInterceptor>());
        collection.AddGrpc(options => options.Interceptors.Add<LoggingInterceptor>());
        collection.AddGrpcReflection();

        return collection;
    }
}
