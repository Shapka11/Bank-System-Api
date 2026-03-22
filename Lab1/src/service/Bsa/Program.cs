using Bsa.Application;
using Bsa.Infrastructure.Persistence;
using Bsa.Presentation.Http;
using FluentMigrator.Runner;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddPresentationHttp();

builder.Services
    .AddSwaggerGen()
    .AddEndpointsApiExplorer()
    .AddOpenApi();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapOpenApi();
app.MapScalarApiReference();

using (IServiceScope scope = app.Services.CreateScope())
{
    IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.UseRouting();
app.UsePresentationHttp();

await app.RunAsync();