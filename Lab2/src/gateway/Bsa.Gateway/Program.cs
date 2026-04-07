using Bsa.Gateway.Application.Extensions;
using Bsa.Gateway.Infrastructure.BankService.Extensions;
using Bsa.Gateway.Middlewares;
using Bsa.Gateway.Presentation.Http.Extensions;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructureService()
    .AddPresentationHttp();

builder.Services
    .AddSwaggerGen()
    .AddEndpointsApiExplorer()
    .AddOpenApi();

builder.Services.AddSingleton<GrpcExceptionMiddleware>();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseRouting();
app.UsePresentationHttp();
app.UseMiddleware<GrpcExceptionMiddleware>();

app.Run();