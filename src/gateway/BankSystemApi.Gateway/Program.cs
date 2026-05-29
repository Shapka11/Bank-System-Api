using BankSystemApi.Gateway.Application.Extensions;
using BankSystemApi.Gateway.Infrastructure.ApprovalService.Extensions;
using BankSystemApi.Gateway.Infrastructure.BankService.Extensions;
using BankSystemApi.Gateway.Middlewares;
using BankSystemApi.Gateway.Presentation.Http.Extensions;
using Extensions;
using Serilog;
using Serilog.Enrichers.ActivityTags;
using Serilog.Enrichers.Span;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

Log.Logger = new LoggerConfiguration()
    .WriteTo.OpenTelemetry()
    .WriteTo.Console()
    .Enrich.WithActivityTags()
    .Enrich.WithSpan()
    .CreateLogger();

builder.Services.AddLogging(logging => logging.ClearProviders().AddSerilog());

builder.Services
    .AddApplication()
    .AddInfrastructureBankService()
    .AddInfrastructureApprovalService()
    .AddPresentationHttp();

builder.Services
    .AddEndpointsApiExplorer()
    .AddOpenApi();

builder.Services.AddSingleton<GrpcExceptionMiddleware>();

builder.Services
    .AddServerSettingsAuthentication(builder.Configuration)
    .AddServerSettingsAuthorization();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwagger(builder.Configuration);

builder.Services.AddMemoryCache();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Infrastructure:Caching:Redis:ConnectionString"];
});

builder.Services.AddHybridCache();

WebApplication app = builder.Build();

app.MapPrometheusScrapingEndpoint();
app.UseSwagger();
app.UseSwaggerUI(swagger =>
{
    swagger.OAuthClientId(builder.Configuration["Authentication:ClientId"] + "-swagger");
    swagger.OAuthUsePkce();
});

app.MapOpenApi();

app.UseRouting();

app.UseMiddleware<GrpcExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UsePresentationHttp();

app.MapDefaultEndpoints();

app.Run();