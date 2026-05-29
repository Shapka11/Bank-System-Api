using BankSystemApi.Application;
using BankSystemApi.Application.Abstractions.Metrics;
using BankSystemApi.HealthChecks;
using BankSystemApi.Infrastructure.Kafka.Extensions;
using BankSystemApi.Infrastructure.Persistence;
using BankSystemApi.Metrics;
using BankSystemApi.Presentation.Grpc.Extensions;
using BankSystemApi.Presentation.Kafka.Extensions;
using Extensions;
using Itmo.Dev.Platform.Common.Extensions;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.MessagePersistence;
using Itmo.Dev.Platform.MessagePersistence.Postgres.Extensions;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Prometheus.Client.DependencyInjection;
using Serilog;
using Serilog.Enrichers.ActivityTags;
using Serilog.Enrichers.Span;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPlatform(config => config.WithSystemTextJsonConfiguration())
    .AddApplication()
    .AddPersistence()
    .AddPresentationGrpc()
    .AddEventPublishers();

builder.Services.AddPlatformKafka(kafka => kafka
    .ConfigureOptions(builder.Configuration.GetSection("Presentation:Kafka"))
    .AddInfrastructureKafkaProducers(builder.Configuration)
    .AddPresentationKafkaConsumers(builder.Configuration));

builder.Services.AddPlatformMessagePersistence(step => step
    .WithDefaultPublisherOptions("MessagePersistence:Publisher:Default")
    .UsePostgresPersistence(configurator => configurator.ConfigureOptions("MessagePersistence:Postgres")));

if (builder.Configuration.GetValue<bool>("USE_PROMETHEUS_METRICS"))
{
    builder.Services.AddMetricFactory();
    builder.Services.AddSingleton<IServiceMetrics, PrometheusServiceMetrics>();
}
else
{
    builder.Services.AddSingleton<IServiceMetrics, DiagnosticsServiceMetrics>();
}

builder.AddServiceDefaults();

builder.Services
    .AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter(DiagnosticsServiceMetrics.Meter.Name)
        .AddPrometheusExporter())
    .WithTracing(tracing => tracing
        .AddSource("BankSystemApi*")
        .AddNpgsql()
        .AddProcessor(new PostgresTraceSuppressor()));

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.OpenTelemetry()
    .WriteTo.Console()
    .Enrich.WithActivityTags()
    .Enrich.WithSpan()
    .CreateLogger();

builder.Services.AddLogging(logging => logging.ClearProviders().AddSerilog());

builder.Services
    .AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: ["health"]);

WebApplication app = builder.Build();

app.UseRouting();
app.MapDefaultEndpoints();

app.MapPrometheusScrapingEndpoint();
if (builder.Configuration.GetValue<bool>("USE_PROMETHEUS_METRICS"))
{
    app.MapPrometheusScrapingEndpoint();
}

app.UsePresentationGrpc();

await app.RunAsync();