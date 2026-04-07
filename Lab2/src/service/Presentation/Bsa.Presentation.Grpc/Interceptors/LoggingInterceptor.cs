using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Diagnostics;

namespace Bsa.Presentation.Grpc.Interceptors;

public sealed class LoggingInterceptor : Interceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            return continuation(request, context);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "Method {Method} executed in {Elapsed}ms",
                context.Method,
                stopwatch.ElapsedMilliseconds);
        }
    }
}