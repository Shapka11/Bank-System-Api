using BankSystemApi.Application;
using BankSystemApi.Application.Abstractions.Metrics;
using BankSystemApi.HealthChecks;
using BankSystemApi.Infrastructure.Persistence;
using BankSystemApi.Metrics;
using BankSystemApi.Presentation.Grpc.Extensions;
using Extensions;
using Itmo.Dev.Platform.Common.Extensions;
using Npgsql;
using OpenTelemetry.Trace;
using Prometheus.Client.AspNetCore;
using Prometheus.Client.DependencyInjection;
using Serilog;
using Serilog.Enrichers.ActivityTags;
using Serilog.Enrichers.Span;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPlatform(config => config.WithSystemTextJsonConfiguration())
    .AddApplication()
    .AddPersistence()
    .AddPresentationGrpc();

if (builder.Configuration.GetValue<bool>("USE_PROMETHEUS_METRICS"))
{
    builder.Services.AddMetricFactory();
    builder.Services.AddSingleton<IServiceMetrics, PrometheusServiceMetrics>();
}
else
{
    builder.Services.AddSingleton<IServiceMetrics, DiagnosticsServiceMetrics>();
}

builder.Services
    .AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter(DiagnosticsServiceMetrics.Meter.Name)
        .AddNpgsqlInstrumentation())
    .WithTracing(tracing => tracing
        .AddSource("BankSystemApi*")
        .AddNpgsql()
        .AddProcessor(new PostgresTraceSuppressor()));

builder.AddServiceDefaults();

Log.Logger = new LoggerConfiguration()
    .WriteTo.OpenTelemetry()
    .WriteTo.Console()
    .Enrich.WithActivityTags()
    .Enrich.WithSpan()
    .CreateLogger();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter(DiagnosticsServiceMetrics.Meter.Name));

builder.Services.AddLogging(logging => logging.ClearProviders().AddSerilog());

builder.Services
    .AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: ["health"]);

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

if (builder.Configuration.GetValue<bool>("USE_PROMETHEUS_METRICS"))
{
    app.UsePrometheusServer();
}

app.UseRouting();
app.UsePresentationGrpc();

await app.RunAsync();