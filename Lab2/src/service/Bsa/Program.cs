using Bsa.Application;
using Bsa.Infrastructure.Persistence;
using Bsa.Presentation.Grpc.Extensions;
using Itmo.Dev.Platform.Common.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPlatform(config => config.WithSystemTextJsonConfiguration())
    .AddApplication()
    .AddPersistence()
    .AddPresentationGrpc();

WebApplication app = builder.Build();

app.UseRouting();
app.UsePresentationGrpc();

await app.RunAsync();