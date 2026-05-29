using AutoBogus;
using BankSystemApi.Domain.ValueObjects;
using Grpc.Net.Client;
using Itmo.Dev.Platform.Testing.ApplicationFactories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;

namespace IntegrationalTests.Fixtures;

public sealed class WebApplicationFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:latest").Build();

    private WebApplicationFactory<Program>? _webApplicationFactory;

    public WebApplicationFactory<Program> Factory =>
        _webApplicationFactory ??
        throw new InvalidOperationException("The factory has not yet been initialized. Wait for InitializeAsync().");

    public IServiceProvider Services => Factory.Services;

    public async Task InitializeAsync()
    {
        InstallFakerConfig();

        Bogus.DataSets.Date.SystemClock = () => DateTime.UtcNow;
        await _container.StartAsync();

        _webApplicationFactory = new PlatformWebApplicationBuilder<Program>()
            .ConfigureConfiguration(builder => builder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Infrastructure:Persistence:Postgres:Host", _container.Hostname },
                {
                    "Infrastructure:Persistence:Postgres:Port",
                    _container.GetMappedPublicPort(5432).ToString()
                },
                { "Infrastructure:Persistence:Postgres:Database", "postgres" },
                { "Infrastructure:Persistence:Postgres:Username", "postgres" },
                { "Infrastructure:Persistence:Postgres:Password", "postgres" },
                { "Infrastructure:Persistence:Postgres:SslMode", "Disable" },
            }))
            .Build();

        _webApplicationFactory.StartServer();
        ReloadNpgsqlTypesAsync(_webApplicationFactory);
    }

    public async Task DisposeAsync()
    {
        if (_webApplicationFactory is not null)
        {
            await _webApplicationFactory.DisposeAsync();
        }

        await _container.DisposeAsync();
    }

    public GrpcChannel CreateChannel()
    {
        var grpcChannelOptions = new GrpcChannelOptions
        {
            HttpHandler = Factory.Server.CreateHandler(),
        };

        return GrpcChannel.ForAddress("http://localhost", grpcChannelOptions);
    }

    private void InstallFakerConfig()
    {
        AutoFaker.Configure(builder =>
        {
            builder.WithOverride<Money>(faker => new Money(faker.Faker.Finance.Amount()));
        });
    }

    private async void ReloadNpgsqlTypesAsync(WebApplicationFactory<Program> webApplicationFactory)
    {
        NpgsqlDataSource? dataSource = webApplicationFactory.Services.GetService<NpgsqlDataSource>();
        if (dataSource is not null)
        {
            await dataSource.ReloadTypesAsync();
        }
        else
        {
            await using var connection = new NpgsqlConnection(_container.GetConnectionString());
            await connection.OpenAsync();
            await connection.ReloadTypesAsync();
        }
    }
}